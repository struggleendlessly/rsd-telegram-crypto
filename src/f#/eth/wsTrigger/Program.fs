namespace wsTrigger

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

open telemetryOption
open AlchemyOptionModule
open ChainSettingsOptionModule
open configurationExtensions
open workers
open telegramOption
open alchemy
open dbMigration
open ethCommonDB

module Program =

    [<EntryPoint>]
    let main args =
        let builder = Host.CreateApplicationBuilder(args)
        //string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        //string strWorkPath = Path.GetDirectoryName(strExeFilePath);

        //builder.
        //    Configuration.
        //    SetBasePath(strWorkPath).
        //    AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        let strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        let strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);

        builder.Configuration
            .SetBasePath(strWorkPath)
            .AddJsonFile("appsettings.json", optional = true, reloadOnChange = true)
            .AddEnvironmentVariables() |> ignore

        let connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        builder.Services.AddDbContext<IEthDB, ethDB>(
            (fun options -> options.UseSqlServer(connectionString)|> ignore),
            ServiceLifetime.Transient) 
            |> ignore

        let openTelemetryOptions = builder.Configuration.GetSection($"{telemetryOption.SectionName}").Get<telemetryOption>()

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
            logger: ILogger<alchemyEVM>, 
            alchemyOptions: IOptions<AlchemyOption>, 
            chainSettingsOption: IOptions<ChainSettingsOption>,
            httpClientFactory: IHttpClientFactory
            ) =
                let instance = alchemyEVM(logger, alchemyOptions,chainSettingsOption, httpClientFactory)
                instance.chainName <- optionsAlchemy.Etherium
                instance

        builder.Services.AddScoped<alchemyEVM>(fun sp ->
            let logger = sp.GetRequiredService<ILogger<alchemyEVM>>()
            let alchemyOptions = sp.GetRequiredService<IOptions<AlchemyOption>>()
            let chainSettingsOption = sp.GetRequiredService<IOptions<ChainSettingsOption>>()
            let httpClientFactory = sp.GetRequiredService<IHttpClientFactory>()
            configureAlchemy(logger, alchemyOptions, chainSettingsOption, httpClientFactory)) |> ignore

        configureServices builder.Services builder.Configuration
        let telegramOption = builder.Configuration.GetSection($"{telegramOption.SectionName}").Get<telegramOption>()
        builder.Services.AddHostedService<trigger_5mins>() |> ignore

        builder.Services.AddWindowsService(fun options -> options.ServiceName <- openTelemetryOptions.ServiceName ) |> ignore

        builder.Build().Run()

        0 // exit code