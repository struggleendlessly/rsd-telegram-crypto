module scopedSwapsBlock

open System
open System.Threading
open System.Linq
open System.IO

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open System.Text
open System.Security.Cryptography

open IScopedProcessingService
open Extensions
open responseSwap
open createSeq

open System.Text.Json
open Microsoft.Extensions.Options
open ChainSettingsOptionModule
open dbMigration
open dbMigration.models
open ApiCallerSOL
open responseGetBlockSol

type SwapToken = 
    { 
    mutable tokenAddress: string
    mutable t0addr: string 
    mutable t1addr: string 
    mutable from: string 
    mutable to_: string 
    mutable t0amount: float 
    mutable t1amount: float 
    mutable t0decimals: int 
    mutable t1decimals: int 
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
    t0amount = 0.0
    t1amount = 0.0
    t0decimals = 0
    t1decimals = 0
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

    let md5 (input : string) : string =
        let data = Encoding.UTF8.GetBytes(input)
        use md5 = MD5.Create()
        (StringBuilder(), md5.ComputeHash(data))
        ||> Array.fold (fun sb b -> sb.Append(b.ToString("x2")))
        |> string    

    let filterSwaps (responses: responseGetBlockSol[]) =
        let mutable resMap = Map.empty<string,(Instruction list * TokenBalance list * TokenBalance list)>

        for response in responses do
            for transaction in response.result.transactions do
                for innerInstruction in transaction.meta.innerInstructions do
                    let mutable filteredInnInst = List.empty<Instruction>
                    let metaHash = JsonSerializer.Serialize(transaction.meta) |> md5

                    for instruction in innerInstruction.instructions do
                        if String.Equals(instruction.program, "spl-token", StringComparison.OrdinalIgnoreCase)                    
                        then                             
                                match instruction.parsed with
                                |Some t when String.Equals(t.``type``, "transferChecked", StringComparison.OrdinalIgnoreCase) || 
                                             String.Equals(t.``type``, "transfer", StringComparison.OrdinalIgnoreCase) 
                                             -> 
                                                filteredInnInst <- instruction :: filteredInnInst
                                |_ -> ()

                    if filteredInnInst |> List.length >= 2 && 
                       transaction.meta.err = None 
                    then
                        resMap <- Map.add transaction.transaction.signatures[0] ((filteredInnInst |> List.rev), transaction.meta.preTokenBalances, transaction.meta.postTokenBalances)resMap

        resMap 
        |> Map.toArray 


    let parceInstructionsTransferChecked (instructions: Instruction list) =
        let swapToken = emptySwapTokens

        if List.length instructions = 2 then
            match List.nth instructions 0, List.nth instructions 1 with
            | instr1, instr2 -> 
                swapToken.t0amount <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> tokenAmount.uiAmount)) |> Option.defaultValue 0.0
                swapToken.t1amount <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> tokenAmount.uiAmount)) |> Option.defaultValue 0.0

                swapToken.t0decimals <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> tokenAmount.decimals)) |> Option.defaultValue 0
                swapToken.t1decimals <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> tokenAmount.decimals)) |> Option.defaultValue 0

                swapToken.t0addr <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.mint) |> Option.defaultValue ""
                swapToken.t1addr <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.mint) |> Option.defaultValue ""

                swapToken.from <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.authority) |> Option.defaultValue ""
                swapToken.to_ <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.authority) |> Option.defaultValue ""

                //if String.Equals(swapToken.t0addr, chainSettingsOption.AddressChainCoin)
                //then
                //    swapToken.priceTokenInSol <- swapToken.t0amount / swapToken.t1amount
                //    swapToken.isBuyToken <- true
                //elif String.Equals(swapToken.t1addr, chainSettingsOption.AddressChainCoin)
                //then
                //    swapToken.priceTokenInSol <- swapToken.t1amount / swapToken.t0amount
                //    swapToken.isBuySol <- true

                //if String.Equals(swapToken.t0addr, chainSettingsOption.AddressStableCoin)
                //then
                //    swapToken.priceTokenInSol <- swapToken.t0amount / swapToken.t1amount
                //    swapToken.isBuyToken <- true
                //elif String.Equals(swapToken.t1addr, chainSettingsOption.AddressStableCoin)
                //then
                //    swapToken.priceTokenInSol <- swapToken.t1amount / swapToken.t0amount
                //    swapToken.isBuySol <- true
    
        swapToken

    let parceInstructionsTransfer(instructions: Instruction list) =
        let swapToken = emptySwapTokens

        if List.length instructions = 2 then
            match List.nth instructions 0, List.nth instructions 1 with
            | instr1, instr2 -> 
                swapToken.t0amount <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> tokenAmount.uiAmount)) |> Option.defaultValue 0.0
                swapToken.t1amount <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> tokenAmount.uiAmount)) |> Option.defaultValue 0.0

                //swapToken.t0decimals <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> tokenAmount.decimals)) |> Option.defaultValue 0
                //swapToken.t1decimals <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.tokenAmount |> Option.map (fun tokenAmount -> tokenAmount.decimals)) |> Option.defaultValue 0

                //swapToken.t0addr <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.mint) |> Option.defaultValue ""
                //swapToken.t1addr <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.mint) |> Option.defaultValue ""

                //swapToken.from <- instr1.parsed |> Option.bind (fun parsed -> parsed.info.authority) |> Option.defaultValue ""
                //swapToken.to_ <- instr2.parsed |> Option.bind (fun parsed -> parsed.info.authority) |> Option.defaultValue ""
    
        swapToken

    let parceInstructions (instructions:Instruction list ) = 
        let res = 
            match instructions 
                  |> List.forall (fun instr -> instr.parsed 
                                               |> Option.exists (fun parsed -> parsed.info.tokenAmount.IsSome)) with
            | true -> parceInstructionsTransferChecked instructions
            | false -> parceInstructionsTransfer instructions
        res
    let processSwaps (d:(string * (Instruction list * TokenBalance list * TokenBalance list))[]) =
        for (signature, (instructions, preTokenBalances, postTokenBalances)) in d do
            let a = parceInstructions instructions
            printfn "%A" signature
        ""

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                let a = File.ReadAllText("C:\Users\strug\Downloads\Untitled.json")
                let b = a |> JsonSerializer.Deserialize<responseGetBlockSol[]>
                let t =     
                        (getSeqToProcessUint64 1UL (uint64 chainSettingsOption.BlocksIn5Minutes) getLastKnownProcessedBlock getLastSolSlot)
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
