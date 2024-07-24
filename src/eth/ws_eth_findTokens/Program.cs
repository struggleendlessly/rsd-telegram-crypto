using api_alchemy.Eth;
using Microsoft.EntityFrameworkCore;

using Shared;
using Shared.ConfigurationOptions;
using Shared.Telegram;
using Data;

using ws_eth_findTokens;
using eth_shared;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

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
builder.Services.AddTransient<FindTransactionService>();

var host = builder.Build();
host.Run();
