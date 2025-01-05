module scopedLastBlock

open System
open System.Linq
open System.Threading

open Microsoft.Extensions.Logging
open Microsoft.Extensions.Options
open Microsoft.EntityFrameworkCore



open dbMigration
open ChainSettingsOptionModule
open IScopedProcessingService
open ApiCallerSOL



type scopedLastBlock(
        logger: ILogger<scopedLastBlock>,
        alchemy: alchemySOL,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        ethDB: solDB) =



    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)


                return ()
            }
