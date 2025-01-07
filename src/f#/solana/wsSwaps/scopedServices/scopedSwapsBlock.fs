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

open createSeq
open Extensions
open ApiCallerSOL
open responseGetBlockSol
open IScopedProcessingService
open ChainSettingsOptionModule

type SwapToken = 
    { 
    mutable tokenAddress: string
    mutable t0addr: string 
    mutable t1addr: string 
    mutable from: string 
    mutable to_: string 
    mutable t0amountFloat: float 
    mutable t1amountFloat: float 
    mutable t0amountInt: uint64 
    mutable t1amountInt: uint64 
    mutable t0decimals: uint64 
    mutable t1decimals: uint64 
    mutable priceTokenInSol: float 
    mutable priceSolInUsd: float 
    mutable isBuyToken: bool
    mutable isBuySol: bool
    }
let emptySwapTokens = { 
    tokenAddress = ""
    t0addr = ""
    t1addr = ""
    from = ""
    to_ = ""
    t0amountFloat = 0.0
    t1amountFloat = 0.0
    t0amountInt = 0UL
    t1amountInt = 0UL
    t0decimals = 0UL
    t1decimals = 0UL
    priceTokenInSol = 0.0 
    priceSolInUsd = 0.0
    isBuyToken = false
    isBuySol = false
    }
type scopedSwapsBlock(
        logger: ILogger<scopedSwapsBlock>,
        alchemy: alchemySOL,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        ethDB: solDB) =

    let chainSettingsOption = chainSettingsOption.Value;

    let getLastKnownProcessedBlock () =
        let noBlock = swapsTokens.Default(chainSettingsOption.DefaultBlockNumber)
        let getNumberInt (x: swapsTokens) = x.blockNumberEndInt

        ethDB.swapsTokensEntities
             .OrderByDescending(fun x -> x.blockNumberEndInt)
             .FirstOrDefaultAsync()
             |> Async.AwaitTask
             |> Async.map (Option.ofObj 
                                >> Option.defaultValue noBlock
                                >> getNumberInt)

    let getLastSolSlot () =
        ethDB.slotsEntities
                .OrderByDescending(fun x -> x.numberInt)
                .FirstOrDefaultAsync()
                |> Async.AwaitTask
                |> Async.map (fun x -> x.numberInt)

    //let md5 (input : string) : string =
    //    let data = Encoding.UTF8.GetBytes(input)
    //    use md5 = MD5.Create()
    //    (StringBuilder(), md5.ComputeHash(data))
    //    ||> Array.fold (fun sb b -> sb.Append(b.ToString("x2")))
    //    |> string   
        
    let list1andLast (lst: 'a list) =
        match lst with
        | [] -> []
        | [x] -> []
        | head :: tail -> 
            let last = List.last tail
            [head; last]

    let splitList list = 
        let half = List.length list / 2 
        let firstHalf = list |> List.take half 
        let secondHalf = list |> List.skip half 
        (firstHalf, secondHalf)

    let filterSwaps (responses: responseGetBlockSol[]) =
        let mutable resMap = Map.empty<string,(Instruction list * TokenBalance list * TokenBalance list)>

        for response in responses do
            for transaction in response.result.transactions do
                for innerInstruction in transaction.meta.innerInstructions do
                    let mutable filteredInnInstTransferChecked = List.empty<Instruction>
                    let mutable filteredInnInstTransfer = List.empty<Instruction>

                    for instruction in innerInstruction.instructions do
                        if String.Equals(instruction.program, "spl-token", StringComparison.OrdinalIgnoreCase)                    
                        then                             
                                match instruction.parsed with
                                |Some t when String.Equals(t.``type``, "transferChecked", StringComparison.OrdinalIgnoreCase) || 
                                             String.Equals(t.``type``, "transfer", StringComparison.OrdinalIgnoreCase) 
                                             -> 
                                                if String.Equals(t.``type``, "transferChecked", StringComparison.OrdinalIgnoreCase)
                                                then
                                                    filteredInnInstTransferChecked <- instruction :: filteredInnInstTransferChecked
                                                else
                                                     filteredInnInstTransfer <- instruction :: filteredInnInstTransfer
                                | _ -> ()
                    if List.length filteredInnInstTransfer = 1 
                    then
                        logger.LogInformation("skip")  // scam
                    
                    if List.length filteredInnInstTransferChecked = 3 
                    then
                        filteredInnInstTransferChecked <-
                            filteredInnInstTransferChecked
                            |> list1andLast 

                    let filteredInnInstTransferCheckedChunked = 
                        match filteredInnInstTransferChecked with
                        | a when List.length filteredInnInstTransferChecked % 2 = 0 -> 
                            filteredInnInstTransferChecked 
                            |> List.toSeq |> Seq.chunkBySize 2 |> Seq.map List.ofSeq |> Seq.toList
                        | _ -> [ ]

                    //let revTC = filteredInnInstTransferChecked 
                    //            |> List.rev 
                    //            |> list1andLast 

                    //let revT =  filteredInnInstTransfer
                    //            |> List.rev 
                    //            |> list1andLast

                    if transaction.meta.err = None && not (List.isEmpty filteredInnInstTransferChecked)
                    then
                        filteredInnInstTransferCheckedChunked
                        |> List.iter (fun instr ->
                            resMap <- Map.add 
                                            transaction.transaction.signatures[0] 
                                            (instr |> List.rev, transaction.meta.preTokenBalances, transaction.meta.postTokenBalances) 
                                            resMap    
                                            )
                                            
                    //if transaction.meta.err = None && not (List.isEmpty revT)
                    //then
                    //        resMap <- Map.add 
                    //                        transaction.transaction.signatures[0] 
                    //                        (revT, transaction.meta.preTokenBalances, transaction.meta.postTokenBalances) 
                    //                        resMap  

        resMap 
        |> Map.toArray 


    let parceInstructionsTransferChecked (instructions: Instruction list) =
        let swapToken = { emptySwapTokens with to_ = "" }

        if List.length instructions = 2 then
            match List.nth instructions 0, List.nth instructions 1 with
            | instr1, instr2 -> 
                swapToken.t0amountFloat <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> tokenAmount.uiAmount)) |> Option.defaultValue 0.0
                swapToken.t1amountFloat <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> tokenAmount.uiAmount)) |> Option.defaultValue 0.0

                swapToken.t0decimals <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> uint64 tokenAmount.decimals)) |> Option.defaultValue 0UL
                swapToken.t1decimals <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> uint64 tokenAmount.decimals)) |> Option.defaultValue 0UL

                swapToken.t0addr <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.mint) |> Option.defaultValue ""
                swapToken.t1addr <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.mint) |> Option.defaultValue ""

                swapToken.from <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.authority) |> Option.defaultValue ""
                swapToken.to_ <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.authority) |> Option.defaultValue ""

        swapToken

    let absDiff a b = if a > b then a - b else b - a
    let compareArrays (pre: TokenBalance list) (post: TokenBalance list) diff =
        let results = 
            post 
            |> List.filter (fun itemPost -> 
                pre |> List.exists (fun itemPre -> absDiff itemPost.uiTokenAmount.amount itemPre.uiTokenAmount.amount = diff))

        results |> List.tryHead

    let parceInstructionsTransfer(instructions: Instruction list) (pre: TokenBalance list) (post: TokenBalance list)  =
        let swapToken = { emptySwapTokens with to_ = "" }

        if List.length instructions = 2 then
            match List.item 0 instructions, List.item 1 instructions with
            | instr1, instr2 -> 
                swapToken.t0amountInt <- 
                            match instr1.parsed with
                            | Some parsed -> parsed.info.amount
                            | None -> 0UL
                swapToken.t1amountInt <- 
                            match instr2.parsed with
                            | Some parsed -> parsed.info.amount
                            | None -> 0UL

                swapToken.from <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.authority) |> Option.defaultValue ""
                swapToken.to_ <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.authority) |> Option.defaultValue ""
        
                let t0 = compareArrays pre post swapToken.t0amountInt
                let t1 = compareArrays pre post swapToken.t1amountInt

                swapToken.t0decimals <- 
                            match t0 with
                            | Some parsed -> parsed.uiTokenAmount.decimals
                            | None -> 0UL

                swapToken.t1decimals <- 
                            match t1 with
                            | Some parsed -> parsed.uiTokenAmount.decimals
                            | None -> 0UL

                swapToken.t0addr <- 
                            match t0 with
                            | Some parsed -> parsed.mint
                            | None -> ""

                swapToken.t1addr <- 
                            match t1 with
                            | Some parsed -> parsed.mint
                            | None -> ""

                swapToken.t0amountFloat <- float swapToken.t0amountInt / (10.0 ** float swapToken.t0decimals)
                swapToken.t1amountFloat <- float swapToken.t1amountInt / (10.0 ** float swapToken.t1decimals)

        swapToken

    let parceInstructions (instructions:Instruction list ) (pre: TokenBalance list) (post: TokenBalance list) = 
        let res = 
            match instructions 
                  |> List.forall (fun instr -> instr.parsed 
                                               |> Option.exists (fun parsed -> parsed.info.tokenAmount.IsSome)) with
            | true -> parceInstructionsTransferChecked instructions
            | false -> parceInstructionsTransfer instructions pre post
        res

    let additionalCalculationsSwaps (swapToken: SwapToken) = 
        if String.Equals(swapToken.t0addr, chainSettingsOption.AddressChainCoin)
        then
            swapToken.priceTokenInSol <- swapToken.t0amountFloat / swapToken.t1amountFloat
            swapToken.isBuyToken <- true
        elif String.Equals(swapToken.t1addr, chainSettingsOption.AddressChainCoin)
        then
            swapToken.priceTokenInSol <- swapToken.t1amountFloat / swapToken.t0amountFloat
            swapToken.isBuySol <- true

        if String.Equals(swapToken.t0addr, chainSettingsOption.AddressStableCoin)
        then
            swapToken.priceTokenInSol <- swapToken.t0amountFloat / swapToken.t1amountFloat
            swapToken.isBuyToken <- true
        elif String.Equals(swapToken.t1addr, chainSettingsOption.AddressStableCoin)
        then
            swapToken.priceTokenInSol <- swapToken.t1amountFloat / swapToken.t0amountFloat
            swapToken.isBuySol <- true

        swapToken
    

    let processSwaps (d:(string * (Instruction list * TokenBalance list * TokenBalance list))[]) =
         d 
         |> Array.map (fun (signature, (instructions, preTokenBalances, postTokenBalances)) -> parceInstructions instructions preTokenBalances postTokenBalances )
         |> Array.map additionalCalculationsSwaps   

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                let a = File.ReadAllText("C:\Users\strug\Downloads\Untitled.json")
                let b = a |> JsonSerializer.Deserialize<responseGetBlockSol[]>
                let t =     
                        (getSeqToProcessUint64 10UL (uint64 chainSettingsOption.BlocksIn5Minutes) getLastKnownProcessedBlock getLastSolSlot)
                        |> Async.Bind alchemy.getBlock 
                        |> Async.map (Array.collect id) 
                        |> Async.map filterSwaps   
                        |> Async.map processSwaps
                        //|> Async.map processArrayAsync
                        //|> Async.Bind processBlocks
                        //|> Async.map saveToDB
                        |> Async.RunSynchronously 
                return ()
            }
