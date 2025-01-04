namespace wsSwaps

open System
open System.Collections.Generic
open System.Net.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging

open AppSettingsOptionModule
open OpenTelemetryOptionModule
open AlchemyOptionModule
open ChainSettingsOptionModule

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
open scopedTokenInfo
open scopedNames

open Microsoft.EntityFrameworkCore;
open dbMigration
open Polly.Timeout
open System.Net
open Microsoft.Extensions.Options
open ethCommonDB

module Program =

    let uncurry2 f = fun x y -> f (x, y)
    let exponentially = float >> (uncurry2 Math.Pow 2) >> TimeSpan.FromSeconds

    let getRetryPolicy() : IAsyncPolicy<HttpResponseMessage> =
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .OrResult(fun msg -> msg.StatusCode = HttpStatusCode.NotFound || msg.StatusCode = HttpStatusCode.TooManyRequests)
            .OrResult(fun msg -> 
                let json = msg.Content.ReadAsStringAsync().Result.Contains(":429,")
                json
            )
            .WaitAndRetryAsync(
                6,
                fun retryAttempt -> TimeSpan.FromSeconds(Math.Pow(2.0, float retryAttempt))
            )

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
            builder.Services.AddDbContext<IEthDB, ethDB>(
                (fun options -> options.UseSqlServer(connectionString)|> ignore),
                ServiceLifetime.Transient) 
                |> ignore

            builder.Services.Configure<AppSettingsOption>(builder.Configuration.GetSection(AppSettingsOption.SectionName)) |> ignore
            builder.Services.Configure<OpenTelemetryOption>(builder.Configuration.GetSection(OpenTelemetryOption.SectionName)) |> ignore
            builder.Services.Configure<AlchemyOption>(builder.Configuration.GetSection(AlchemyOption.SectionName)) |> ignore
            builder.Services.Configure<ChainSettingsOption>(builder.Configuration.GetSection(ChainSettingsOption.SectionName)) |> ignore

            let aet = builder.Configuration.GetSection($"{ChainSettingsOption.SectionName}").Get<ChainSettingsOption>()

            let openTelemetryOptions = builder.Configuration.GetSection($"{OpenTelemetryOption.SectionName}").Get<OpenTelemetryOption>()

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
                .AddPolicyHandler(getRetryPolicy())
                .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, fun retryAttempt -> exponentially retryAttempt)) |> ignore

            let optionsAlchemy = builder.Configuration.GetSection($"{AlchemyOption.SectionName}:ChainNames").Get<ChainNames>()

            let configureAlchemy(
                logger: ILogger<alchemy>, 
                alchemyOptions: IOptions<AlchemyOption>, 
                chainSettingsOption: IOptions<ChainSettingsOption>,
                httpClientFactory: IHttpClientFactory
                ) =
                    let instance = alchemy(logger, alchemyOptions,chainSettingsOption, httpClientFactory)
                    instance.chainName <- optionsAlchemy.Base
                    instance

            builder.Services.AddScoped<alchemy>(fun sp ->
                let logger = sp.GetRequiredService<ILogger<alchemy>>()
                let alchemyOptions = sp.GetRequiredService<IOptions<AlchemyOption>>()
                let chainSettingsOption = sp.GetRequiredService<IOptions<ChainSettingsOption>>()
                let httpClientFactory = sp.GetRequiredService<IHttpClientFactory>()
                configureAlchemy(logger, alchemyOptions, chainSettingsOption, httpClientFactory)) |> ignore


            //builder.Services.AddScoped<alchemy>() |> ignore
            builder.Services.AddScoped<scopedTokenInfo>() |> ignore
            builder.Services.AddScoped<scopedSwapsETH>() |> ignore
            builder.Services.AddScoped<scopedSwapsTokens>() |> ignore
            builder.Services.AddScoped<scopedLastBlock>() |> ignore
            //let a = seq { 10..6..44 }
            builder.Services.AddScoped<IDictionary<string, IScopedProcessingService>>(
                fun sp -> 
                    let dict = new Dictionary<string, IScopedProcessingService>() 
                    dict.Add(scopedSwapsETH, sp.GetRequiredService<scopedSwapsETH>() :> IScopedProcessingService)
                    dict.Add(scopedSwapsTokens, sp.GetRequiredService<scopedSwapsTokens>() :> IScopedProcessingService) 
                    dict.Add(scopedLastBlock, sp.GetRequiredService<scopedLastBlock>() :> IScopedProcessingService) 
                    dict :> IDictionary<string, IScopedProcessingService> ) |> ignore

            //builder.Services.AddHostedService<swapsETH>() |> ignore
            builder.Services.AddHostedService<swapsTokens>() |> ignore
            //builder.Services.AddHostedService<lastBlock>() |> ignore

            builder.Services.AddWindowsService(fun options -> options.ServiceName <- "ws_base_swaps" ) |> ignore

            builder.Build().Run()

            0 // exit code
         finally
            Log.CloseAndFlush()