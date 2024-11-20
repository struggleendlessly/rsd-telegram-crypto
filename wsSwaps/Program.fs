namespace wsSwaps

open System
open System.Collections.Generic
open System.Linq
open System.Net.Http
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration
open AppSettingsOptionModule
open OpenTelemetryOptionModule
open Polly
open Polly.Extensions.Http
open Serilog
open Serilog.Sinks.OpenTelemetry
open Serilog.Formatting.Compact

module Program =

    let uncurry2 f = fun x y -> f (x, y)

    let exponentially = float >> (uncurry2 Math.Pow 2) >> TimeSpan.FromSeconds

    [<EntryPoint>]
    let main args =

        try
            Log.Information("Starting up")

            let builder = Host.CreateApplicationBuilder(args)
            
            builder.Configuration
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional = true, reloadOnChange = true)
                .AddEnvironmentVariables() |> ignore

            builder.Services.Configure<AppSettingsOption>(builder.Configuration.GetSection(AppSettingsOption.SectionName)) |> ignore
            builder.Services.Configure<OpenTelemetryOption>(builder.Configuration.GetSection(OpenTelemetryOption.SectionName)) |> ignore
            let openTelemetryOptions = builder.Configuration.Get<OpenTelemetryOption>()

            Log.Logger <- LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.Console(RenderedCompactJsonFormatter())
                                .WriteTo.OpenTelemetry(fun x ->
                                    x.Endpoint <- openTelemetryOptions.Url
                                    x.Protocol <- OtlpProtocol.HttpProtobuf
                                    x.Headers <- dict [ "X-Seq-ApiKey", openTelemetryOptions.ApiKey ]
                                    x.ResourceAttributes <- dict [ "service.name", openTelemetryOptions.ServiceName ]
                                )
                                .CreateLogger()

            builder.Services.AddHttpClient("Api")  
                .ConfigurePrimaryHttpMessageHandler(
                    Func<HttpMessageHandler>(
                        fun _ -> new SocketsHttpHandler(MaxConnectionsPerServer = 1000)
                    ))
                .SetHandlerLifetime(TimeSpan.FromMinutes 5.0)
                .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, fun retryAttempt -> exponentially retryAttempt)) |> ignore

            builder.Services.AddHostedService<Worker>() |> ignore

            // Add Serilog
            //builder.Logging. |> ignore

            let configuration = builder.Configuration
            let appSettings = configuration.Get<AppSettingsOption>()
            let mySetting = appSettings.Logging.LogLevel.Default

            printfn "MySetting: %s" mySetting

            builder.Build().Run()

            0 // exit code
         finally
            Log.CloseAndFlush()