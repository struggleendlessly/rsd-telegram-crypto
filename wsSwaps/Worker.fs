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
                let numbers = seq { 121235670 .. 121235680 }
                //let aa = prepareChunks numbers
                let ee = alchemy.getBlockByNumber() numbers 
                //logger.LogInformation("Response from Alchemy: {@ee}", ee)
                //use client = httpClientFactory.CreateClient("Api")

                //let! response = client.GetAsync "https://3908dca0d72445af90b8a3060008df171.api.mockbin.io" |> Async.AwaitTask 
                //let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask 
                //let googleResponse = JsonSerializer.Deserialize<GoogleResponse>(content)

                //printfn "Response from Google: %s" content

                // we run synchronously
                // to allow the fsi to finish the pending tasks

                do! Task.Delay(1000)
        }
