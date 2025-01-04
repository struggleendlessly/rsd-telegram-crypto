module scopedLastBlock

open System
open System.Linq
open System.Threading

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open IScopedProcessingService

open alchemy
open ethCommonDB
open ethCommonDB.models

open createSeq
open Extensions
open responseGetBlock
open mapResponseGetBlock

type BlockDetectionResult = 
    | NewBlocks of responseGetBlocks[] 
    | NoNewBlocks

type scopedLastBlock(
        logger: ILogger<scopedLastBlock>,
        alchemy: alchemy,
        ethDB: IEthDB) as this =

    let getLastKnownBlockInDB  =
        let noBlock = EthBlocksEntity.Default(24567082)
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

    let saveToDB blocks = 
        async {
            if Array.isEmpty blocks then
                return 0
            else
                do! ethDB.EthBlocksEntities.AddRangeAsync(blocks) |> Async.AwaitTask
                let! result = ethDB.SaveChangesAsync() |> Async.AwaitTask
                return result
        }

    let getBlocks n startBlock = 
         getSeqToProcess1 n startBlock
         >> Async.Bind alchemy.getBlockByNumber  
         >> Async.map mapBlocks
         >> Async.Bind saveToDB

    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let lastKnownBlock = getLastKnownBlockInDB
                let res = getBlocks 1000
                                getLastKnownBlockInDB
                                getLastEthBlock
                          |> Async.RunSynchronously

                return res
            }
