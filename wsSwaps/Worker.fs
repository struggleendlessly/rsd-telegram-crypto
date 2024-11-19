namespace wsSwaps

open System
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open AppSettingsOption
open Microsoft.Extensions.Options
open System.Net.Http

type Worker(
    logger: ILogger<Worker>, 
    settings: IOptions<AppSettings>, 
    httpClientFactory: IHttpClientFactory
    ) =
    inherit BackgroundService()

    override _.ExecuteAsync(ct: CancellationToken) =
        task {
            while not ct.IsCancellationRequested do
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                logger.LogInformation("Logging level: {level}", settings.Value.Logging.LogLevel.Default)

                let client = httpClientFactory.CreateClient("Api")
                let response = client.GetAsync("https://www.google.com").Result
                let content = response.Content.ReadAsStringAsync().Result
                printfn "Response from Google: %s" content

                do! Task.Delay(1000)
        }
