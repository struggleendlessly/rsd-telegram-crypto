namespace wsSwaps

open System
open System.Collections.Generic
open System.Linq
open System.Net.Http
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open AppSettingsOptionModule
open OpenTelemetryOptionModule
open AlchemyOptionModule
open responseGetBlockDTO
open Polly
open Polly.Extensions.Http
open Serilog
open Serilog.Sinks.OpenTelemetry
open Serilog.Formatting.Compact
open alchemy
open UlrBuilder
open System.Text.Json
open IScopedProcessingService
open MyScopedProcessingService
open Microsoft.EntityFrameworkCore;
open dbMigration

module Program =

    let uncurry2 f = fun x y -> f (x, y)

    let exponentially = float >> (uncurry2 Math.Pow 2) >> TimeSpan.FromSeconds

    //let myCustomFunction (logger: ILogger) =
    //    logger.Information("My custom function is called")
    //let prepareChunks numbers =        
    //     numbers  
    //     |> Seq.chunkBySize 5 
    //     |> Seq.mapi (fun index value -> index, value |> Seq.map getBlockByNumber |> JsonSerializer.Serialize) 
    //     |> Seq.iter (fun (index, value) -> printfn "Index: %d, Value:%s" index  value  )

    //     printfn "Index:"
    //     ""
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
            builder.Services.AddScoped<MyScopedProcessingService>() |> ignore

            builder.Services.AddScoped<IDictionary<string, IScopedProcessingService>>(
                fun sp -> 
                    let dict = new Dictionary<string, IScopedProcessingService>() 
                    dict.Add("WorkerService1", sp.GetRequiredService<MyScopedProcessingService>() :> IScopedProcessingService) 

                    dict :> IDictionary<string, IScopedProcessingService> ) |> ignore

            builder.Services.AddHostedService<Worker>() |> ignore

            let configuration = builder.Configuration
            let appSettings = configuration.Get<AppSettingsOption>()
            let mySetting = appSettings.Logging.LogLevel.Default

            printfn "MySetting: %s" mySetting

            builder.Build().Run()

            0 // exit code
         finally
            Log.CloseAndFlush()