namespace wsSwaps

open System
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

open OpenTelemetryOptionModule
open AlchemyOptionModule
open ChainSettingsOptionModule
open configurationExtensions

open alchemy
open dbMigration
open ethCommonDB

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
            builder.Services.AddDbContext<IEthDB, ethDB>(
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

            configureServices builder.Services builder.Configuration

            //builder.Services.AddHostedService<swapsETH>() |> ignore
            builder.Services.AddHostedService<swapsTokens>() |> ignore
            //builder.Services.AddHostedService<lastBlock>() |> ignore

            builder.Services.AddWindowsService(fun options -> options.ServiceName <- openTelemetryOptions.ServiceName ) |> ignore

            builder.Build().Run()

            0 // exit code
         finally
            Log.CloseAndFlush()