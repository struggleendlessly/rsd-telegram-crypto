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

type trigger5min = 
    | MKless100k of string
    | MKmore100kLess1m of string
    | MKmore1m of string

type scoped_telegram(
        logger: ILogger<scoped_telegram>,
        telegramOption:  IOptions<telegramOption>,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        httpClientFactory: IHttpClientFactory,
        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;
    let telemetryOption = telegramOption.Value;

    let mapTriggerToMessage_5mins (x:triggerResults)= 

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

        let mk = BigDecimal.Parse(x.mkStr.Replace(".", ""))

        if mk <= BigDecimal.Parse("100000") 
        then
            MKless100k res
        elif mk > BigDecimal.Parse("100000") && mk <= BigDecimal.Parse("1000000") 
        then
            MKmore100kLess1m res
        else
            MKmore1m res

    let mapTriggerToMessage_0volumeNperiods (x:triggerResults)= 

        let sb = StringBuilder()
        sb.Append($"{x.nameLong} / {x.nameShort}") |> ignore

        sb.Append("\n") |> ignore
        sb.Append($"`{x.pairAddress}`") |> ignore

        sb.Append("\n") |> ignore
        sb.Append($"MK: {x.mkStr} USD") |> ignore

        sb.Append("\n") |> ignore
        sb.Append($"TS: {x.totalSupplyStr}") |> ignore

        sb.Append("\n") |> ignore
        sb.Append($"Buy:{x.ethInUsdSumStr} / Sell:{x.ethOutUsdSumStr} / Total: {x.ethOutInUsdSumStr}"  ) |> ignore
        sb.Append(" USD") |> ignore

        sb.Append("\n") |> ignore
        sb.Append($"""{apiCallerTLGRM.icons["chart"]} [dextools]({telemetryOption.dextoolsUrl}app/en/{telemetryOption.chainName}/pair-explorer/{x.pairAddress}""")  |> ignore

        let res =  sb.ToString()

        res

    member this.sendMessages_trigger_5min = 
        Seq.map mapTriggerToMessage_5mins
        >> Seq.map ( fun x ->
                match x with
                | MKless100k res -> apiCallerTLGRM.urlBuilder (telemetryOption.message_thread_id_5mins_less100k|> string) (telemetryOption.chat_id_coins|> string) res
                | MKmore100kLess1m res -> apiCallerTLGRM.urlBuilder (telemetryOption.message_thread_id_5mins_more100kLess1m|> string) (telemetryOption.chat_id_coins|> string) res 
                | MKmore1m res -> apiCallerTLGRM.urlBuilder (telemetryOption.message_thread_id_5mins_more1m|> string) (telemetryOption.chat_id_coins|> string) res
                )
        >> Seq.map (apiCallerTLGRM.request logger telemetryOption.bot_hash telemetryOption.UrlBase httpClientFactory)
        >> Async.Parallel

    member this.sendMessages_trigger_0volumeNperiods = 
        Seq.map mapTriggerToMessage_0volumeNperiods
        >> Seq.map ( fun x ->
                apiCallerTLGRM.urlBuilder (telemetryOption.message_thread_id_0volumeNperiods|> string) (telemetryOption.chat_id_coins|> string) x
                )
        >> Seq.map (apiCallerTLGRM.request logger telemetryOption.bot_hash telemetryOption.UrlBase httpClientFactory)
        >> Async.Parallel

    member this.sendMessages_trigger_5mins5percOfMK = 
        Seq.map mapTriggerToMessage_0volumeNperiods
        >> Seq.map ( fun x ->
                apiCallerTLGRM.urlBuilder (telemetryOption.message_thread_id_5min_5percOfMK|> string) (telemetryOption.chat_id_coins|> string) x
                )
        >> Seq.map (apiCallerTLGRM.request logger telemetryOption.bot_hash telemetryOption.UrlBase httpClientFactory)
        >> Async.Parallel

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)


                return ()
            }