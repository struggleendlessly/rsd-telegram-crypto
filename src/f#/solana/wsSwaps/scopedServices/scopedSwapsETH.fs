module scopedSwapsETH

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open IScopedProcessingService
open Extensions
open responseSwap

open alchemy

open Microsoft.Extensions.Options
open ChainSettingsOptionModule
open dbMigration
open ApiCallerSOL

type scopedSwapsETH(
        logger: ILogger<scopedSwapsETH>,
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
