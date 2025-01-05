module scopedTokenInfo

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open ChainSettingsOptionModule
open IScopedProcessingService



open Microsoft.Extensions.Options

open dbMigration
open ApiCallerSOL


type scopedTokenInfo(
        logger: ILogger<scopedTokenInfo>,
        alchemy: alchemySOL,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        ethDB: solDB) =

    let chainSettingsOption = chainSettingsOption.Value;

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                    

                return ()
            }