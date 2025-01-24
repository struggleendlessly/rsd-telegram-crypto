module scoped_telegram

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open ChainSettingsOptionModule
open IScopedProcessingService
open Extensions
open responseSwap
open telemetryOption
open apiCallerTLGRM 
open telegramOption

open alchemy
open Microsoft.Extensions.Options
open ethCommonDB
open ethCommonDB.models
open Nethereum.Util
open bl_others
open System.Net.Http
open System.Numerics
open System.Globalization
open System.Text

type scoped_telegram(
        logger: ILogger<scoped_telegram>,
        telegramOption:  IOptions<telegramOption>,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        httpClientFactory: IHttpClientFactory,
        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;
    let telemetryOption = telegramOption.Value;

    let mapTriggerToMessage (x:triggerResults)= 

        let sb = StringBuilder()
        sb.Append($"{x.nameLong} / {x.nameShort}") |> ignore

        sb.Append("\n") |> ignore
        sb.Append($"`{x.pairAddress}`") |> ignore

        sb.Append("\n") |> ignore
        sb.Append($"MK: {x.mkStr} USD") |> ignore

        sb.Append("\n") |> ignore
        sb.Append($"TS: {x.totalSupplyStr}") |> ignore

        sb.Append("\n") |> ignore
        sb.Append($"{x.priceDifferenceStr}") |> ignore
        sb.Append(" X") |> ignore

        sb.Append("\n") |> ignore
        sb.Append($"Buy:{x.ethInUsdSumStr} / Sell:{x.ethOutUsdSumStr} / Total: {x.ethOutInUsdSumStr}"  ) |> ignore
        sb.Append(" USD") |> ignore

        sb.Append("\n") |> ignore
        sb.Append($"""{apiCallerTLGRM.icons["chart"]} [dextools]({telemetryOption.dextoolsUrl}app/en/{telemetryOption.chainName}/pair-explorer/{x.pairAddress}""")  |> ignore

        let res =  sb.ToString()

        res

    member this.sendMessages = 
        Array.map mapTriggerToMessage
        >> Array.map (apiCallerTLGRM.urlBuilder (telemetryOption.message_thread_id_5mins|> string) (telemetryOption.chat_id_coins|> string) )
        >> Array.map (apiCallerTLGRM.request logger telemetryOption.bot_hash telemetryOption.UrlBase httpClientFactory)
        >> Async.Parallel

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)


                return ()
            }