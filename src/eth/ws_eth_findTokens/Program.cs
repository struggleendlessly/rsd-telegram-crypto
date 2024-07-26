using api_alchemy.Eth;

using Data;

using eth_shared;
using eth_shared.Processors;

using Microsoft.EntityFrameworkCore;

using nethereum;

using Serilog;
using Serilog.Sinks.OpenTelemetry;

using Shared;
using Shared.ConfigurationOptions;

using ws_eth_findTokens;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration().
    Enrich.FromLogContext().

    WriteTo.Console().
    WriteTo.OpenTelemetry(
    x =>
    {
        x.Endpoint = "http://localhost:5341";
        x.Protocol = OtlpProtocol.HttpProtobuf;
        x.Headers = new Dictionary<string, string>
        {
            ["Authorization"] = "KoRAGAVQfLnnJNet9VRj"
        };
        x.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = "ws_eth_findTokens"
        };
    }).
    CreateLogger();

builder.Logging.AddSerilog();
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "ws_eth_findTokens";
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<dbContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);

builder.Services.Configure<OptionsAlchemy>(builder.Configuration.GetSection(OptionsAlchemy.SectionName));

builder.Services.AddHttpClient("ApiAlchemy", client =>
{
})
.ConfigurePrimaryHttpMessageHandler(() =>
    {
        var socketHandler = new SocketsHttpHandler
        {
            MaxConnectionsPerServer = int.MaxValue,
            PooledConnectionLifetime = TimeSpan.FromMinutes(15),
        };
        return socketHandler;
    })
.SetHandlerLifetime(TimeSpan.FromMinutes(5))
.AddPolicyHandler(PolicyHandlers.GetRetryPolicy());

builder.Services.AddTransient<EthApi>();
builder.Services.AddTransient<ApiWeb3>();
builder.Services.AddTransient<ProcessorGeneral>();
builder.Services.AddTransient<FindTransactionService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
