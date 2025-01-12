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

type scoped_telegram(
        logger: ILogger<scoped_telegram>,
        telegramOption:  IOptions<telegramOption>,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        httpClientFactory: IHttpClientFactory,
        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;
    let telemetryOption = telegramOption.Value;

                        //case "public":
                        //item.messageText =
                        //    item.line_tokenName + " \n" +
                        //    item.line_tokenAddress + " \n" +

                        //    $"{icons["clockSend"]} {average.periodInMins} mins   \n" +
                        //    $"{buyToSell} Now buy:  {(decimal)average.last.volumePositiveEth:0.##} {item.currency}  Sell:  {(decimal)average.last.volumeNegativeEth:0.##} {item.currency}  \n" +
                        //    $"{icons["antenna"]} Market Cap: {marketCapStr} \n" +
                        //    $"{icons["chart"]} [dextools]({optionsTelegram.dextoolsUrl}app/en/ether/pair-explorer/{item.pairAddress}) " +
                        //    $"{icons["chart"]} [dexscreener]({optionsTelegram.dexscreenerUrl}ethereum/{item.pairAddress})";

    let mapTriggerToMessage (x:triggerResults)= 
        let aa = x.priceDifference.RoundAwayFromZero 1
        let res =  sprintf "%s\n%s\n%s"
                            x.pairAddress
                            (x.priceDifference.RoundAwayFromZero(1)|> string)
                            (string x.volumeInUsd)


        res

    member this.mapTriggersToMessage (x:triggerResults [] )= 
        let a = 
            x |> Array.map mapTriggerToMessage
            |> Array.map (apiCallerTLGRM.urlBuilder (telemetryOption.message_thread_id_5mins|> string) (telemetryOption.chat_id_coins|> string) )
            |> Array.map (apiCallerTLGRM.request logger telemetryOption.bot_hash telemetryOption.UrlBase httpClientFactory)
            |> Async.Parallel
            |> Async.RunSynchronously

        a


    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)


                return ()
            }