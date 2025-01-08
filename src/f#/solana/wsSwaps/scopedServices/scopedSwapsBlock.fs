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
        
    let filterSwaps (responses: responseGetBlockSol[]) =
        let mutable resMap = Map.empty<string,(uint64 * Instruction list * TokenBalance list * TokenBalance list)>

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
                                            (response.result.parentSlot, instr |> List.rev, transaction.meta.preTokenBalances, transaction.meta.postTokenBalances) 
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


    let parceInstructionsTransferChecked slotNuber (instructions: Instruction list) =
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

                swapToken.slotNuber <- slotNuber

        swapToken

    //let absDiff a b = if a > b then a - b else b - a
    //let compareArrays (pre: TokenBalance list) (post: TokenBalance list) diff =
    //    let results = 
    //        post 
    //        |> List.filter (fun itemPost -> 
    //            pre |> List.exists (fun itemPre -> absDiff itemPost.uiTokenAmount.amount itemPre.uiTokenAmount.amount = diff))

    //    results |> List.tryHead

    //let parceInstructionsTransfer(instructions: Instruction list) (pre: TokenBalance list) (post: TokenBalance list)  =
    //    let swapToken = { emptySwapTokens with to_ = "" }

    //    if List.length instructions = 2 then
    //        match List.item 0 instructions, List.item 1 instructions with
    //        | instr1, instr2 -> 
    //            swapToken.t0amountInt <- 
    //                        match instr1.parsed with
    //                        | Some parsed -> parsed.info.amount
    //                        | None -> 0UL
    //            swapToken.t1amountInt <- 
    //                        match instr2.parsed with
    //                        | Some parsed -> parsed.info.amount
    //                        | None -> 0UL

    //            swapToken.from <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.authority) |> Option.defaultValue ""
    //            swapToken.to_ <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.authority) |> Option.defaultValue ""
        
    //            let t0 = compareArrays pre post swapToken.t0amountInt
    //            let t1 = compareArrays pre post swapToken.t1amountInt

    //            swapToken.t0decimals <- 
    //                        match t0 with
    //                        | Some parsed -> parsed.uiTokenAmount.decimals
    //                        | None -> 0UL

    //            swapToken.t1decimals <- 
    //                        match t1 with
    //                        | Some parsed -> parsed.uiTokenAmount.decimals
    //                        | None -> 0UL

    //            swapToken.t0addr <- 
    //                        match t0 with
    //                        | Some parsed -> parsed.mint
    //                        | None -> ""

    //            swapToken.t1addr <- 
    //                        match t1 with
    //                        | Some parsed -> parsed.mint
    //                        | None -> ""

    //            swapToken.t0amountFloat <- float swapToken.t0amountInt / (10.0 ** float swapToken.t0decimals)
    //            swapToken.t1amountFloat <- float swapToken.t1amountInt / (10.0 ** float swapToken.t1decimals)

    //    swapToken

    let parceInstructions slotNuber (instructions:Instruction list ) (pre: TokenBalance list) (post: TokenBalance list) = 
        let res = 
            match instructions 
                  |> List.forall (fun instr -> instr.parsed 
                                               |> Option.exists (fun parsed -> parsed.info.tokenAmount.IsSome)) with
            | true -> parceInstructionsTransferChecked slotNuber instructions
           // | false -> parceInstructionsTransfer instructions pre post
            | false -> { emptySwapTokens with to_ = "" }
        res

    //let additionalCalculationsSwaps (swapToken: SwapToken) = 
    //    if String.Equals(swapToken.t0addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase)
    //    then
    //        swapToken.priceTokenInSol <- swapToken.t0amountFloat / swapToken.t1amountFloat
    //        swapToken.isBuyToken <- true
    //    elif String.Equals(swapToken.t1addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase)
    //    then
    //        swapToken.priceTokenInSol <- swapToken.t1amountFloat / swapToken.t0amountFloat
    //        swapToken.isBuySol <- true

    //    if String.Equals(swapToken.t0addr, chainSettingsOption.AddressStableCoin, StringComparison.InvariantCultureIgnoreCase)       
    //    then
    //        swapToken.priceSolInUsd <- swapToken.t0amountFloat / swapToken.t1amountFloat
    //        swapToken.isBuyToken <- true
    //    elif String.Equals(swapToken.t1addr, chainSettingsOption.AddressStableCoin, StringComparison.InvariantCultureIgnoreCase)
    //    then
    //        swapToken.priceSolInUsd <- swapToken.t1amountFloat / swapToken.t0amountFloat
    //        swapToken.isBuySol <- true

    //    swapToken

    let processSwaps (d:(string * (uint64 * Instruction list * TokenBalance list * TokenBalance list))[]) =
         d 
         |> Array.map (fun (signature, (slotNumber ,instructions, preTokenBalances, postTokenBalances)) -> parceInstructions slotNumber instructions preTokenBalances postTokenBalances )
         //|> Array.map additionalCalculationsSwaps  

    let filterStableCoins (swapToken: SwapToken) =

        if (String.Equals(swapToken.t0addr, chainSettingsOption.AddressStableCoin, StringComparison.InvariantCultureIgnoreCase) &&
            String.Equals(swapToken.t1addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase))
            ||
           (String.Equals(swapToken.t0addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase) &&
            String.Equals(swapToken.t1addr, chainSettingsOption.AddressStableCoin, StringComparison.InvariantCultureIgnoreCase))
        then
            if String.Equals(swapToken.t0addr, chainSettingsOption.AddressStableCoin, StringComparison.InvariantCultureIgnoreCase)       
            then
                swapToken.priceSolInUsd <- swapToken.t0amountFloat / swapToken.t1amountFloat
                swapToken.isBuyToken <- true
            elif String.Equals(swapToken.t1addr, chainSettingsOption.AddressStableCoin, StringComparison.InvariantCultureIgnoreCase)
            then
                swapToken.priceSolInUsd <- swapToken.t1amountFloat / swapToken.t0amountFloat
                swapToken.isBuySol <- true

            Some (StableCoin swapToken)
        elif //(String.Equals(swapToken.t0addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase) &&
              chainSettingsOption.ExcludedAddresses |> Array.exists (fun item -> String.Equals(item, swapToken.t1addr, StringComparison.InvariantCultureIgnoreCase))
              ||
             //(String.Equals(swapToken.t1addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase) &&
              chainSettingsOption.ExcludedAddresses |> Array.exists (fun item -> String.Equals(item, swapToken.t0addr, StringComparison.InvariantCultureIgnoreCase))
        then
            None
        else
            if String.Equals(swapToken.t0addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase)
            then
                swapToken.priceTokenInSol <- swapToken.t0amountFloat / swapToken.t1amountFloat
                swapToken.isBuyToken <- true
            elif String.Equals(swapToken.t1addr, chainSettingsOption.AddressChainCoin, StringComparison.InvariantCultureIgnoreCase)
            then
                swapToken.priceTokenInSol <- swapToken.t1amountFloat / swapToken.t0amountFloat
                swapToken.isBuySol <- true

            Some (Token swapToken)
    
    let saveToDB blocks = 
        async {
            if Array.isEmpty blocks then
                return 0
            else
                do! solDB.swapsTokensEntities.AddRangeAsync(blocks) |> Async.AwaitTask
                let! result = solDB.SaveChangesAsync() |> Async.AwaitTask
                return result
        }      

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                //let a = File.ReadAllText("C:\Users\strug\Downloads\Untitled.json")
                //let b = a |> JsonSerializer.Deserialize<responseGetBlockSol[]>
                let seq = getSeqToProcessUint64 10UL (uint64 chainSettingsOption.BlocksIn5Minutes) getLastKnownProcessedBlock getLastSolSlot
                let! seqX = seq
                let startBlock = Seq.head seqX
                let endBlock = Seq.last seqX

                let t =     
                        seq
                        |> Async.Bind alchemy.getBlock 
                        |> Async.map (Array.collect id) 
                        |> Async.map filterSwaps   
                        |> Async.map processSwaps
                        |> Async.map (Array.map filterStableCoins)
                        |> Async.map (mapSwapTokens startBlock endBlock)
                        //|> Async.map saveToDB
                        |> Async.RunSynchronously 
                return ()
            }
           