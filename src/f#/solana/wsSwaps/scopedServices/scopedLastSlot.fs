module scopedLastBlock

open System
open System.Linq
open System.Threading

open Microsoft.Extensions.Logging
open Microsoft.Extensions.Options

open dbMigration
open Extensions
open ChainSettingsOptionModule
open IScopedProcessingService
open ApiCallerSOL

open mapGetSlot

type scopedLastSlot(
        logger: ILogger<scopedLastSlot>,
        alchemy: alchemySOL,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        solDB: solDB) =

    let saveToDB slot = 
        async {
            let! _ = solDB.slotsEntities.AddAsync(slot).AsTask() |> Async.AwaitTask
            let! result = solDB.SaveChangesAsync() |> Async.AwaitTask
            return result
        }

    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let t = alchemy.getLastSlot()
                            |> Async.map mapToEntity
                            |> Async.Bind saveToDB
                            |> Async.RunSynchronously

                return ()
            }
