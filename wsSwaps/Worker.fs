namespace wsSwaps

open System
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open AppSettingsOptionModule
open Microsoft.Extensions.Options
open System.Net.Http
open System.Text.Json
open alchemy
open UlrBuilder

type GoogleResponse = { 
    message: string
}

type Worker(
    logger: ILogger<Worker>, 
    settings: IOptions<AppSettingsOption>, 
    httpClientFactory: IHttpClientFactory,
    alchemy: alchemy
    ) =
    inherit BackgroundService()
    override _.ExecuteAsync(ct: CancellationToken) =
        task {
            while not ct.IsCancellationRequested do
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                logger.LogInformation("Logging level: {level}", settings.Value.Logging.LogLevel.Default)

                let numbers = seq { 21252610 .. 21252620 } |> Seq.toArray

                alchemy.ShuffleApiKeys()
                //let blocks = alchemy.getBlockByNumber() numbers 
                let lastBlock = alchemy.getLastBlockNumber() 

                do! Task.Delay(1000)
        }