module scoped_trigger_5mins

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


type scoped_trigger_5mins(
        logger: ILogger<scoped_trigger_5mins>,
        alchemy: alchemyEVM,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;

    let getTokens0and1 (addressPair: string[]) =
        ethDB.tokenInfoEntities
            .Where(fun x -> addressPair.Contains(x.AddressPair))
            .ToListAsync()
            |> Async.AwaitTask


    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                    

                return ()
            }