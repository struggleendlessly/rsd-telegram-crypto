namespace wsSwaps

open System
open System.Collections.Generic
open System.Net.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration

open AppSettingsOptionModule
open OpenTelemetryOptionModule
open AlchemyOptionModule

open Polly
open Polly.Extensions.Http

open Serilog
open Serilog.Sinks.OpenTelemetry
open Serilog.Formatting.Compact

open alchemy

open IScopedProcessingService
open scopedSwapsETH
open scopedLastBlock
open scopedSwapsTokens

open Microsoft.EntityFrameworkCore;
open dbMigration
open Extensions
open System.Numerics
open ExtendedNumerics
open System.Globalization

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

            let connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            builder.Services.AddDbContext<ethDB>(
                (fun options -> options.UseSqlServer(connectionString)|> ignore),
                ServiceLifetime.Transient) 
                |> ignore

            builder.Services.Configure<AppSettingsOption>(builder.Configuration.GetSection(AppSettingsOption.SectionName)) |> ignore
            builder.Services.Configure<OpenTelemetryOption>(builder.Configuration.GetSection(OpenTelemetryOption.SectionName)) |> ignore
            builder.Services.Configure<AlchemyOption>(builder.Configuration.GetSection(AlchemyOption.SectionName)) |> ignore
            let openTelemetryOptions = builder.Configuration.Get<OpenTelemetryOption>()

            Log.Logger <- LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.Console(new RenderedCompactJsonFormatter())
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

            builder.Services.AddScoped<alchemy>() |> ignore
            builder.Services.AddScoped<scopedSwapsETH>() |> ignore
            builder.Services.AddScoped<scopedSwapsTokens>() |> ignore
            builder.Services.AddScoped<scopedLastBlock>() |> ignore
            //let a = seq { 10..6..44 }
            builder.Services.AddScoped<IDictionary<string, IScopedProcessingService>>(
                fun sp -> 
                    let dict = new Dictionary<string, IScopedProcessingService>() 
                    dict.Add("scopedSwapsETH", sp.GetRequiredService<scopedSwapsETH>() :> IScopedProcessingService)
                    dict.Add("scopedSwapsTokens", sp.GetRequiredService<scopedSwapsTokens>() :> IScopedProcessingService) 
                    dict.Add("scopedLastBlock", sp.GetRequiredService<scopedLastBlock>() :> IScopedProcessingService) 
                    dict :> IDictionary<string, IScopedProcessingService> ) |> ignore

            //builder.Services.AddHostedService<swapsETH>() |> ignore
            builder.Services.AddHostedService<swapsTokens>() |> ignore
            //builder.Services.AddHostedService<lastBlock>() |> ignore

            builder.Services.AddWindowsService(fun options -> options.ServiceName <- "ws_eth_findTokens" ) |> ignore

            let asyncTask1 = async { return 10 }
            let asyncTask2 x = async { return x * 2 }

            let boundTask = Async.Bind asyncTask2 asyncTask1

            let result = Async.RunSynchronously boundTask

            printfn "Result: %d" result // Output: 20


            let configuration = builder.Configuration
            let appSettings = configuration.Get<AppSettingsOption>()
            let mySetting = appSettings.Logging.LogLevel.Default

            printfn "MySetting: %s" mySetting

            builder.Build().Run()

            0 // exit code
         finally
            Log.CloseAndFlush()