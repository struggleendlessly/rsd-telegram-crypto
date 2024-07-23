using api_alchemy.Eth;
using Shared;
using Shared.ConfigurationOptions;
using Shared.Telegram;

using ws_eth_findTokens;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<OptionsAlchemy>(builder.Configuration.GetSection(OptionsAlchemy.SectionName));

builder.Services.AddHttpClient("ApiAlchemy", client =>
{
})
.SetHandlerLifetime(TimeSpan.FromMinutes(5))
.AddPolicyHandler(PolicyHandlers.GetRetryPolicy());

builder.Services.AddTransient<EthApi>();

var host = builder.Build();
host.Run();
