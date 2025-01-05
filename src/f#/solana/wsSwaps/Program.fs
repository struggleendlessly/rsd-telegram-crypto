namespace wsSwaps

open System
open System.Net
open System.Net.Http

open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Options
open Microsoft.EntityFrameworkCore;
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection

open Serilog
open Serilog.Sinks.OpenTelemetry
open Serilog.Formatting.Compact

open Polly
open Polly.Timeout
open Polly.Extensions.Http

open AppSettingsOptionModule
open OpenTelemetryOptionModule
open AlchemyOptionModule
open ChainSettingsOptionModule
open dbMigration
open scopedSwapsTokens
open ApiCallerSOL
open System.Collections.Generic
open IScopedProcessingService
open scopedTokenInfo
open scopedLastBlock
open scopedSwapsETH
open scopedNames


module Program =

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

    let uncurry2 f = fun x y -> f (x, y)
    let exponentially = float >> (uncurry2 Math.Pow 2) >> TimeSpan.FromSeconds

    [<EntryPoint>]
    let main args =
        let builder = Host.CreateApplicationBuilder(args)

        builder.Configuration
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional = true, reloadOnChange = true)
            .AddEnvironmentVariables() |> ignore

        let connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        builder.Services.AddDbContext<solDB>(
            (fun options -> options.UseSqlServer(connectionString)|> ignore),
            ServiceLifetime.Transient) 
            |> ignore


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

        let optionsAlchemy = builder.Configuration.GetSection($"{AlchemyOption.SectionName}:ChainNames").Get<ChainNames>()

        builder.Services.Configure<AppSettingsOption>(builder.Configuration.GetSection(AppSettingsOption.SectionName)) |> ignore
        builder.Services.Configure<OpenTelemetryOption>(builder.Configuration.GetSection(OpenTelemetryOption.SectionName)) |> ignore
        builder.Services.Configure<AlchemyOption>(builder.Configuration.GetSection(AlchemyOption.SectionName)) |> ignore
        builder.Services.Configure<ChainSettingsOption>(builder.Configuration.GetSection(ChainSettingsOption.SectionName)) |> ignore

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
            logger: ILogger<alchemySOL>, 
            alchemyOptions: IOptions<AlchemyOption>, 
            chainSettingsOption: IOptions<ChainSettingsOption>,
            httpClientFactory: IHttpClientFactory
            ) =
                let instance = alchemySOL(logger, alchemyOptions,chainSettingsOption, httpClientFactory)
                instance.chainName <- optionsAlchemy.Solana
                instance

        builder.Services.AddScoped<alchemySOL>(fun sp ->
            let logger = sp.GetRequiredService<ILogger<alchemySOL>>()
            let alchemyOptions = sp.GetRequiredService<IOptions<AlchemyOption>>()
            let chainSettingsOption = sp.GetRequiredService<IOptions<ChainSettingsOption>>()
            let httpClientFactory = sp.GetRequiredService<IHttpClientFactory>()
            configureAlchemy(logger, alchemyOptions, chainSettingsOption, httpClientFactory)) |> ignore



        builder.Services.AddScoped<scopedTokenInfo>() |> ignore
        builder.Services.AddScoped<scopedSwapsETH>() |> ignore
        builder.Services.AddScoped<scopedSwapsTokens>() |> ignore
        builder.Services.AddScoped<scopedLastBlock>() |> ignore
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

        builder.Services.AddWindowsService(fun options -> options.ServiceName <- openTelemetryOptions.ServiceName ) |> ignore
        builder.Build().Run()

        0 // exit code