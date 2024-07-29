using api_alchemy.Eth;

using Data;

using eth_shared;

using etherscan;

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
        x.Endpoint = builder.Configuration.GetSection("OpenTelemetry").GetValue<string>("Url");
        x.Protocol = OtlpProtocol.HttpProtobuf;
        x.Headers = new Dictionary<string, string>
        {

            ["X-Seq-ApiKey"] = builder.Configuration.GetSection("OpenTelemetry").GetValue<string>("ApiKey")
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
builder.Services.Configure<OptionsEtherscan>(builder.Configuration.GetSection(OptionsEtherscan.SectionName));

builder.Services.AddHttpClient("Api", client =>
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

builder.Services.AddTransient<EtherscanApi>();
builder.Services.AddTransient<EthApi>();
builder.Services.AddTransient<ApiWeb3>();
builder.Services.AddTransient<GetBlocks>();
builder.Services.AddTransient<GetTotalSupply>();
builder.Services.AddTransient<GetTransactions>();
builder.Services.AddTransient<GetTokenMetadata>();
builder.Services.AddTransient<GetTransactionReceipt>();
builder.Services.AddTransient<Step1>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
