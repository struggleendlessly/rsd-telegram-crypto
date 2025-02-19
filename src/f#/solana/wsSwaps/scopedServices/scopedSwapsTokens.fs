module scopedSwapsTokens

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open IScopedProcessingService
open ChainSettingsOptionModule
open Extensions
open responseSwap
open bl_createSeq



open alchemy

open Microsoft.Extensions.Options
open dbMigration
open ApiCallerSOL

type scopedSwapsTokens(
        logger: ILogger<scopedSwapsTokens>,
        alchemy: alchemySOL,
        //scopedTokenInfo: scopedTokenInfo,
        //scopedSwapsETH: scopedSwapsETH,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        ethDB: solDB) =

    let chainSettingsOption = chainSettingsOption.Value;


    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) (value: int) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                return ()

            }
