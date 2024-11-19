namespace wsSwaps

open System
open System.Collections.Generic
open System.Linq
open System.Net.Http
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration
open AppSettingsOption
open Polly
open Polly.Extensions.Http

module Program =


    let configureSocketsHttpHandler: Func<HttpMessageHandler>  = Func<HttpMessageHandler>(
        fun () -> new SocketsHttpHandler(MaxConnectionsPerServer = 10)
        )
    let uncurry2 f = fun x y -> f (x, y)

    let exponentially = 
      float 
      >> (uncurry2 Math.Pow 2)
      >> TimeSpan.FromSeconds

    [<EntryPoint>]
    let main args =
        let builder = Host.CreateApplicationBuilder(args)
        
        // Add configuration to the builder
        builder.Configuration
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional = true, reloadOnChange = true)
            .AddEnvironmentVariables() |> ignore

        builder.Services.Configure<AppSettings>(builder.Configuration) |> ignore

        builder.Services.AddHttpClient("Api")  
            .ConfigurePrimaryHttpMessageHandler(
                Func<HttpMessageHandler>(
                     fun _ -> new SocketsHttpHandler(MaxConnectionsPerServer = 10)
                ))
            .SetHandlerLifetime(TimeSpan.FromMinutes 5.)
            .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, fun retryAttempt -> exponentially retryAttempt)) |> ignore

        builder.Services.AddHostedService<Worker>() |> ignore

        let configuration = builder.Configuration
        let appSettings = configuration.Get<AppSettings>()
        let mySetting = appSettings.Logging.LogLevel.Default

        printfn "MySetting: %s" mySetting


        builder.Build().Run()


        0 // exit code