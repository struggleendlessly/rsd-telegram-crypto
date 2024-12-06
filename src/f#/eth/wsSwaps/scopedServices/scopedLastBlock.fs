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

    let getLastKnownBlockInDB  =
        let noBlock = EthBlocksEntity.Default()
        let getNumberInt (x: EthBlocksEntity) = x.numberInt

        ethDB.EthBlocksEntities
                .OrderByDescending(fun x -> x.numberInt)
                .FirstOrDefaultAsync()
                |> Async.AwaitTask
                |> Async.map (Option.ofObj 
                                >> Option.defaultValue noBlock
                                >> getNumberInt)

    let getLastEthBlock = 
        async {
                let! a = alchemy.getLastBlockNumber()                      
                return a.blockInt
        }

    let createSeq1 (start: Async<int>) (end1: Async<int>) = 
        async{
            let! startAsync  = start
            let! endAsync = end1
            
            if endAsync - startAsync > 1000
            then
                return seq { startAsync + 1 .. startAsync + 1000 } |> Seq.toArray
            else 
                return seq { startAsync + 1 .. endAsync } |> Seq.toArray
        }

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

    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let lastKnownBlock = getLastKnownBlockInDB
                let res = getBlocks 
                                getLastKnownBlockInDB
                                getLastEthBlock
                          |> Async.RunSynchronously

                return res
            }
