module scopedLastBlock

open System
open System.Threading
open System.Threading.Tasks
open System.Linq

open Microsoft.Extensions.Logging

open IScopedProcessingService
open dbMigration
open dbMigration.models
open alchemy
open System.Threading.Tasks
open Microsoft.EntityFrameworkCore
open responseGetBlockDTO

type BlockDetectionResult = 
    | NewBlocks of responseGetBlocksDTO[] 
    | NoNewBlocks

type scopedLastBlock(
        logger: ILogger<scopedLastBlock>,
        alchemy: alchemy,
        ethDB: ethDB) as this =

    let getLastKnownBlockInDB = 
        task {
            let! result = 
                ethDB.EthBlocksEntities
                    .OrderByDescending(fun x -> x.numberInt)
                    .FirstOrDefaultAsync()              
            return 
                match isNull result with 
                | true -> EthBlocksEntity.Default() 
                | false -> result
        } |> Async.AwaitTask

    let getBlocks startBlock endBlock = 
        seq { startBlock + 1 .. endBlock } 
        |> Seq.toArray
        |> alchemy.getBlockByNumber         

    let detectNewBlocks  = 
        let firstBlock = getLastKnownBlockInDB |> Async.RunSynchronously
        let lastBlock = alchemy.getLastBlockNumber().blockInt

        match lastBlock > firstBlock.numberInt with
        | true -> 
            logger.LogInformation("New block detected: {lastBlock}", lastBlock) 
            let res = getBlocks firstBlock.numberInt lastBlock
            Some(NewBlocks res) 
        | false -> 
            logger.LogInformation("No new blocks detected") 
            None

    //let processDB = function
    //    | NewBlocks blocks -> 
    //        blocks
    //        |> Array.iter(fun block -> 
    //            let entity = ethDB.EthBlocksEntities.FromBlockDTO(block)
    //            ethDB.EthBlocksEntities.Add(entity) |> ignore
    //        )
    //        ethDB.SaveChangesAsync() |> ignore
    //    | NoNewBlocks -> ()
    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                let e = detectNewBlocks

                let aaa = 
                    match e with
                    | Some(NewBlocks (blocks: responseGetBlocksDTO[])) -> 
                        blocks
                        |> Array.collect id
                    | _ -> Array.empty<responseGetBlockDTO>

                return aaa
                //logger.LogInformation("Last block: {lastBlock}", lastBlock)
            }
