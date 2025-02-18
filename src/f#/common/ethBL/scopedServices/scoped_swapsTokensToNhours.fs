module scoped_swapsTokensToNhours

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open ChainSettingsOptionModule
open IScopedProcessingService
open Extensions
open responseSwap

open alchemy
open Microsoft.Extensions.Options
open ethCommonDB
open ethCommonDB.models
open System.Numerics
open responseEthCall
open System.Globalization
open responseTokenMetadata


type scoped_swapsTokensToNhours(
        logger: ILogger<scoped_swapsTokensToNhours>,
        alchemy: alchemyEVM,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;


    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) (value: int) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                return 0
            }