using api_alchemy;

using Shared;

using ws_eth_findTokens;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ApiAlchemy>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseUrl"]);
})
.SetHandlerLifetime(TimeSpan.FromMinutes(5))
.AddPolicyHandler(PolicyHandlers.GetRetryPolicy());

var host = builder.Build();
host.Run();
