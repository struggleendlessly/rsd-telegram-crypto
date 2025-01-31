module scoped_trigger_0volumeNperiods

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.Options

open ChainSettingsOptionModule
open IScopedProcessingService
open Extensions
open responseSwap
open scoped_telegram
open bl_others

open alchemy
open ethCommonDB
open ethCommonDB.models
open Nethereum.Util
open System.Net.Http

type scoped_trigger_0volumeNperiods(
        logger: ILogger<scoped_trigger_0volumeNperiods>,
        alchemy: alchemyEVM,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        scoped_telegram: scoped_telegram,

        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;

    let getLastProcessedBlock () =
        ethDB.swapsETH_TokenEntities
            .OrderByDescending(fun x -> x.blockNumberEndInt)
            .Take(1)
            .Select(fun x -> x.blockNumberEndInt)
            .FirstOrDefaultAsync()
            |> Async.AwaitTask

    let getLatestTrigger () =
        let noBlock = TriggerHistory.Default()
        let getNumberInt (x: TriggerHistory) = x.blockNumberEndInt

        ethDB.triggerHistoriesEntities
            .Where(fun x -> x.title = scopedNames.trigger_5mins_Name)
            .OrderByDescending(fun x -> x.blockNumberEndInt)
            .Take(1)
            .FirstOrDefaultAsync()
            |> Async.AwaitTask
            |> Async.map (Option.ofObj 
                >> Option.defaultValue noBlock
                >> getNumberInt)

    let getTxnsForPeriod (firstBlockInPeriod) =
        ethDB.swapsETH_TokenEntities
            .Where(fun x -> x.blockNumberEndInt >= firstBlockInPeriod)
            .ToListAsync()
            |> Async.AwaitTask


    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                let countInPeriods = 12

                let! lastBlock = getLastProcessedBlock()
                let! latestTrigger = getLatestTrigger()

                if lastBlock - latestTrigger < chainSettingsOption.BlocksIn5Minutes
                then
                    return ()
                else
                    return ()

            }