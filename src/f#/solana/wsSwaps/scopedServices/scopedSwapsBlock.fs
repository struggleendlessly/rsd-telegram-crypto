module scopedSwapsBlock

open System
open System.Threading
open System.Linq
open System.IO
open System.Text.Json

open Microsoft.Extensions.Logging
open Microsoft.Extensions.Options
open Microsoft.EntityFrameworkCore

open dbMigration
open dbMigration.models

open bl
open bl_others
open bl_createSeq

open mapGetSwaps
open Extensions
open ApiCallerSOL
open responseGetBlockSol
open IScopedProcessingService
open ChainSettingsOptionModule

type transferType = 
    | Transf of Instruction
    | TransfChecked of Instruction

type scopedSwapsBlock(
        logger: ILogger<scopedSwapsBlock>,
        alchemy: alchemySOL,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        solDB: solDB) =

    let chainSettingsOption = chainSettingsOption.Value;

    let getLastKnownProcessedBlock () =
        let noBlock = swapsTokens.Default(chainSettingsOption.DefaultBlockNumber)
        let getNumberInt (x: swapsTokens) = x.slotNumberEndInt

        solDB.swapsTokensEntities
             .OrderByDescending(fun x -> x.slotNumberEndInt)
             .FirstOrDefaultAsync()
             |> Async.AwaitTask
             |> Async.map (Option.ofObj 
                                >> Option.defaultValue noBlock
                                >> getNumberInt)

    let getLastSolSlot () =
        solDB.slotsEntities
                .OrderByDescending(fun x -> x.numberInt)
                .FirstOrDefaultAsync()
                |> Async.AwaitTask
                |> Async.map (fun x -> x.numberInt)

    let getLastSolUsdDefault() =
        let noBlock = swapsTokensUSD.Default()
        let getNumberInt (x: swapsTokensUSD) = x.priceSolInUsd

        solDB.swapsTokensUSDEntities
                .OrderByDescending(fun x -> x.slotNumberInt)
                .FirstOrDefaultAsync()
                |> Async.AwaitTask
                |> Async.map (Option.ofObj 
                                    >> Option.defaultValue noBlock
                                    >> getNumberInt)

    let absDiff a b = if a > b then a - b else b - a
    let compareArrays (pre: TokenBalance list) (post: TokenBalance list) diff =
        let results = 
            post 
            |> List.filter (fun itemPost -> 
                pre |> List.exists (fun itemPre -> absDiff itemPost.uiTokenAmount.amount itemPre.uiTokenAmount.amount = diff))

        results |> List.tryHead

    let mapTransfToSwapToken txn (pre: TokenBalance list) (post: TokenBalance list) v = 
        let amountInt = 
                    match v.parsed with
                    | Some parsed -> parsed.info.amount
                    | None -> 0UL

        let t = compareArrays pre post amountInt
        let decimals = 
                    match t with
                    | Some parsed -> parsed.uiTokenAmount.decimals
                    | None -> 0UL

        let instructionToken = 
            { 
                 address = match t with
                            | Some parsed -> parsed.mint
                            | None -> ""

                 from = v.parsed |> Option.bind (fun parsed -> parsed.info.authority) |> Option.defaultValue "" 
                 to_ = "" 
                 txn = txn
                 amountFloat = float amountInt / (10.0 ** float decimals)
                 amountInt = amountInt
                 decimals = decimals
             }

        instructionToken 
        
    let mapTransCheckedfToSwapToken txn v = 
        let instructionToken = 
            { 
                 address = v.parsed |> Option.bind (fun parsed -> parsed.info.mint) |> Option.defaultValue "" 
                 from = v.parsed |> Option.bind (fun parsed -> parsed.info.authority) |> Option.defaultValue "" 
                 to_ = "" 
                 txn = txn
                 amountFloat =  v.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> tokenAmount.uiAmount)) |> Option.defaultValue 0.0
                 amountInt = 0UL 
                 decimals = v.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> uint64 tokenAmount.decimals)) |> Option.defaultValue 0UL 
             }

        instructionToken

    let mapToInstructionToken txn (pre: TokenBalance list) (post: TokenBalance list) transferType =
        match transferType with
        | Transf v -> mapTransfToSwapToken txn pre post v
        | TransfChecked v -> mapTransCheckedfToSwapToken txn v
    let emptyParsedInfo = {
        lamports = None
        newAccount = None
        owner = None
        source = None
        space = None
        account = None
        mint = None
        systemProgram = None
        tokenProgram = None
        wallet = None
        extensionTypes = None
        authority = None
        amount = 0UL
        destination = None
        tokenAmount = None
        }
    let getParsed (trans: transferType) = 
        match trans with 
            | Transf instr -> instr.parsed |> Option.bind (fun parsed -> Some parsed.info) |> Option.defaultValue emptyParsedInfo 
            | TransfChecked instr -> instr.parsed |> Option.bind (fun parsed -> Some parsed.info) |> Option.defaultValue emptyParsedInfo

    let filterSplitMoney ( lst: transferType list )=  
        if List.length lst > 5
        then
            let infos = List.map getParsed lst
            let a = 
                match infos with
                    | [] -> []
                    | h1 :: h2 :: (hn::tail) -> 
                        if h1.destination = h2.source
                        then
                            lst |> List.skip 1 |> List.take (List.length lst - 2)
                        else
                            lst
                    | _ -> lst
            a
        else
            lst

    let processTransactions (responses: responseGetBlockSol[]) =
        let mutable resMap = List.empty<instructionToken list>

        for response in responses do
            if response.error = None
            then
                for transaction in response.result.transactions do
                    if transaction.meta.err = None
                    then
                        let txn = transaction.transaction.signatures[0]
                        for innerInstruction in transaction.meta.innerInstructions do

                            let mutable filteredInnInstTransfer = List.empty<transferType>
                            for instruction in innerInstruction.instructions do
                                if String.Equals(instruction.program, "spl-token", StringComparison.OrdinalIgnoreCase)                    
                                then                             
                                        match instruction.parsed with
                                        |Some t when String.Equals(t.``type``, "transferChecked", StringComparison.OrdinalIgnoreCase) || 
                                                     String.Equals(t.``type``, "transfer", StringComparison.OrdinalIgnoreCase) 
                                                     ->                                                
                                                        if String.Equals(t.``type``, "transferChecked", StringComparison.OrdinalIgnoreCase)
                                                        then
                                                            filteredInnInstTransfer <- TransfChecked (instruction) :: filteredInnInstTransfer
                                                        else
                                                             let prev = filteredInnInstTransfer |> List.tryHead
                                                             let r = match prev with
                                                                        | Some (Transf prevInstr) when 
                                                                                prevInstr.parsed.Value.info.source.Value.Equals(instruction.parsed.Value.info.source.Value, StringComparison.InvariantCultureIgnoreCase) -> 
                                                                                filteredInnInstTransfer <- Transf (instruction) :: List.tail filteredInnInstTransfer
                                                                        | _ ->  filteredInnInstTransfer <- Transf (instruction) :: filteredInnInstTransfer
                                                             ()
                                                      
                                        | _ -> ()
                                
                            let filteredInnInstTransferChunked = 
                                match filteredInnInstTransfer with
                                | a when List.length filteredInnInstTransfer % 2 = 0 -> 
                                    filteredInnInstTransfer 
                                    |> List.rev 
                                    |> filterSplitMoney
                                    |> List.map (mapToInstructionToken txn transaction.meta.preTokenBalances transaction.meta.postTokenBalances)
                                    |> List.toSeq 
                                    |> Seq.chunkBySize 2 
                                    |> Seq.map List.ofSeq 
                                    |> Seq.toList
                                | _ -> [ ]
                        
                            if not (List.isEmpty filteredInnInstTransferChunked)
                            then
                                filteredInnInstTransferChunked
                                |> List.iter (fun swapTokens ->
                                      resMap <- swapTokens :: resMap )
                                            
        resMap 
        |> List.toArray 


    let mapToSwapToken (instructions: instructionToken list) =
        match List.item 0 instructions, List.item 1 instructions with
        | t0, t1 -> 
            { emptySwapTokens with
                t0addr = t0.address
                t1addr = t1.address
                from = t0.from
                to_ = t1.from
                txn = t0.txn
                t0amountFloat = t0.amountFloat
                t1amountFloat = t1.amountFloat
                t0amountInt = t0.amountInt
                t1amountInt = t1.amountInt
                t0decimals = t0.decimals
                t1decimals = t1.decimals
            }
        | _ -> emptySwapTokens

    let filterSwaps (swapToken: SwapToken) =

        // sol - usdc
        if (String.Equals(swapToken.t0addr, chainSettingsOption.AddressStableCoin, StringComparison.InvariantCultureIgnoreCase) && 
            String.Equals(swapToken.t1addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase))
            ||
           (String.Equals(swapToken.t0addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase) &&
            String.Equals(swapToken.t1addr, chainSettingsOption.AddressStableCoin, StringComparison.InvariantCultureIgnoreCase))
        then 
            if String.Equals(swapToken.t0addr, chainSettingsOption.AddressStableCoin, StringComparison.InvariantCultureIgnoreCase)       
            then
                let x = 
                    { swapToken with 
                        tokenAddress = swapToken.t1addr
                        priceSolInUsd = swapToken.t0amountFloat / swapToken.t1amountFloat
                        isBuyToken = true }
                Some (StableCoin x)
            elif String.Equals(swapToken.t1addr, chainSettingsOption.AddressStableCoin, StringComparison.InvariantCultureIgnoreCase)
            then
                let x = 
                    { swapToken with 
                        tokenAddress = swapToken.t0addr
                        priceSolInUsd = swapToken.t1amountFloat / swapToken.t0amountFloat
                        isBuySol = true }
                Some (StableCoin x)
             else 
                None

        // token - token
        elif  String.Equals(swapToken.t0addr, swapToken.t1addr, StringComparison.InvariantCultureIgnoreCase)
        then 
            None

        // token - stable coin
        elif chainSettingsOption.ExcludedAddresses |> Array.exists (fun item -> String.Equals(item, swapToken.t1addr, StringComparison.InvariantCultureIgnoreCase))
              ||
             chainSettingsOption.ExcludedAddresses |> Array.exists (fun item -> String.Equals(item, swapToken.t0addr, StringComparison.InvariantCultureIgnoreCase))
        then 
            if String.Equals(swapToken.t0addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase) ||
               String.Equals(swapToken.t1addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase)
            then 
                None
            else
                Some (TokenUSD swapToken)

        // token - sol
        else 
            if String.Equals(swapToken.t0addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase)
            then
                let x = 
                    { swapToken with 
                        tokenAddress = swapToken.t1addr
                        priceTokenInSol = swapToken.t0amountFloat / swapToken.t1amountFloat
                        isBuyToken = true }
                Some (TokenSol x)
            elif String.Equals(swapToken.t1addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase)
            then
                let x = 
                    { swapToken with 
                        tokenAddress = swapToken.t0addr
                        priceTokenInSol = swapToken.t1amountFloat / swapToken.t0amountFloat
                        isBuySol = true }
                Some (TokenSol x)

            else
                None
    
    let saveToDB (v: swapsTokens[] * swapsTokensUSD option) = 
        async {
            let swapsTokens, swapsTokensUSD = v

            do! solDB.swapsTokensEntities.AddRangeAsync(swapsTokens) |> Async.AwaitTask

            match swapsTokensUSD with
            | Some swapsTokensUSD -> 
                    let! _ = solDB.swapsTokensUSDEntities.AddAsync(swapsTokensUSD).AsTask() |> Async.AwaitTask
                    ()
            | None -> ()

            let! result = solDB.SaveChangesAsync() |> Async.AwaitTask
            return result
        }      

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                //let a = File.ReadAllText("C:\Users\strug\Downloads\Untitled.json")
                //let b = a |> JsonSerializer.Deserialize<responseGetBlockSol[]>
                let seq = getSeqToProcessUint64 50UL (uint64 chainSettingsOption.BlocksIn5Minutes) getLastKnownProcessedBlock getLastSolSlot
                let! seqX = seq
                let startBlock = Seq.head seqX
                let endBlock = Seq.last seqX
                let! solUsdDefault = getLastSolUsdDefault()
                let t =     
                        seq
                        |> Async.Bind alchemy.getBlock 
                        |> Async.map (Array.collect id) 
                        |> Async.map processTransactions   
                        |> Async.map (Array.map mapToSwapToken)
                        |> Async.map (Array.map filterSwaps)
                        |> Async.map (mapToEnteties chainSettingsOption.ExcludedAddresses solUsdDefault startBlock endBlock)
                        |> Async.Bind saveToDB
                        |> Async.RunSynchronously 

                return ()
            }
           