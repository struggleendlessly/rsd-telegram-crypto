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
open Extensions
open Microsoft.EntityFrameworkCore
open responseGetBlockDTO
open responseGetLastBlockDTO

type BlockDetectionResult = 
    | NewBlocks of responseGetBlocksDTO[] 
    | NoNewBlocks

type scopedLastBlock(
        logger: ILogger<scopedLastBlock>,
        alchemy: alchemy,
        ethDB: ethDB) as this =

    let getLastKnownBlockInDB _= 
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

    let createSeq1 (start: Async<EthBlocksEntity>) (end1:unit -> Async<responseGetLastBlockDTO>) = 
        async{
            let! startAsync  = start
            let! endAsync = end1()
            
            let startBlock = startAsync.numberInt + 1
            let endBlock = endAsync.blockInt

            match endBlock > startBlock with
            | true -> 
                logger.LogInformation("New block detected: {lastBlock}", startBlock) 
                return seq { startBlock .. endBlock } |> Seq.toArray
            | false -> 
                logger.LogInformation("No new blocks detected") 
                return seq { 0.. -1 } |> Seq.toArray
        }

   // let t = createSeq1 getLastKnownBlockInDB alchemy.getLastBlockNumber
    let processBlocks blocks = 
        async {
               return
                    blocks
                    |> Array.collect id
                    |> Array.Parallel.map(fun block -> 
                        block 
                        |> mapResponseGetBlock.map
                    )
            }

    //let saveToDB blocks = 
    //    async {


    //        ethDB.EthBlocksEntities.AddRangeAsync(blocks) |> ignore
    //        return ethDB.SaveChangesAsync() 
    //    }
    let saveToDB blocks = 
        async {
            if Array.isEmpty blocks then
                return 0
            else
                do! ethDB.EthBlocksEntities.AddRangeAsync(blocks) |> Async.AwaitTask
                let! result = ethDB.SaveChangesAsync() |> Async.AwaitTask
                return result
        }

    let getBlocks startBlock = 
         createSeq1 startBlock
         >> Async.Bind alchemy.getBlockByNumber      
         >> Async.Bind processBlocks
         >> Async.Bind saveToDB

    //let detectNewBlocks  = 
    //    let firstBlock = getLastKnownBlockInDB |> Async.RunSynchronously
    //    let lastBlock = alchemy.getLastBlockNumber().blockInt

    //    match lastBlock > firstBlock.numberInt with
    //    | true -> 
    //        logger.LogInformation("New block detected: {lastBlock}", lastBlock) 
    //        let res = getBlocks firstBlock.numberInt lastBlock
    //        Some(NewBlocks res) 
    //    | false -> 
    //        logger.LogInformation("No new blocks detected") 
    //        None

    //let processDB = function
    //    | NewBlocks blocks -> 
    //        blocks
    //        |> Array.Parallel.iter(fun block -> 
    //            let entity = block |> mapResponseGetBlock.map
    //            ethDB.EthBlocksEntities.Add(entity) |> ignore
    //        )
    //        ethDB.SaveChangesAsync() |> ignore
    //    | NoNewBlocks -> ()
    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let lastKnownBlock = getLastKnownBlockInDB ()
                let res = getBlocks 
                            lastKnownBlock
                            alchemy.getLastBlockNumber
                          |> Async.RunSynchronously
                return res
                //let aaa = 
                //    match e with
                //    | blocks -> 
                //        blocks
                //        |> Array.collect id
                //        |> Array.Parallel.map(fun block -> 
                //            block 
                //            |> mapResponseGetBlock.map
                //        )
                //    | _ -> Array.empty<EthBlocksEntity>

                //ethDB.EthBlocksEntities.AddRangeAsync(aaa) |> ignore
                //return ethDB.SaveChangesAsync() 
                //logger.LogInformation("Last block: {lastBlock}", lastBlock)
            }
