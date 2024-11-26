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

type scopedLastBlock(
        logger: ILogger<scopedLastBlock>,
        alchemy: alchemy,
        ethDB: ethDB) =
    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let numbers = seq { 21252610 .. 21252620 } |> Seq.toArray

                let lastKnownBlock = 
                    ethDB.
                        EthBlocksEntities.
                        OrderByDescending(fun x -> x.numberInt).
                        AsEnumerable().
                        DefaultIfEmpty(EthBlocksEntity.Default()).
                        FirstOrDefault()

                alchemy.ShuffleApiKeys()
                let lastBlock = alchemy.getLastBlockNumber() 

                logger.LogInformation("Last block: {lastBlock}", lastBlock)
            }
