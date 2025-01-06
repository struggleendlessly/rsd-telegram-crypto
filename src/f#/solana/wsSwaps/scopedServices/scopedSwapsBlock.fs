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

    let fiilterSwaps (responses: responseGetBlockSol[]) =
        let mutable metaMap = Map.empty<string, Meta>

        for response in responses do
            for transaction in response.result.transactions do
                for innerInstruction in transaction.meta.innerInstructions do
                    for instruction in innerInstruction.instructions do
                        if String.Equals(instruction.program, "spl-token", StringComparison.OrdinalIgnoreCase)                    
                        then                             
                                match instruction.parsed with
                                |Some t when String.Equals(t.``type``, "transferChecked", StringComparison.OrdinalIgnoreCase) || 
                                             String.Equals(t.``type``, "transfer", StringComparison.OrdinalIgnoreCase) 
                                             -> 
                                               let metaHash = JsonSerializer.Serialize(transaction.meta) |> md5
                                               metaMap <- Map.add metaHash transaction.meta metaMap
                                |_ -> ()

        metaMap 
        |> Map.toArray 
        |> Array.map (fun (k, v) -> v) 

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
                        |> Async.map fiilterSwaps                       
                        //|> Async.map processArrayAsync
                        //|> Async.Bind processBlocks
                        //|> Async.map saveToDB
                        |> Async.RunSynchronously 
                return ()
            }
