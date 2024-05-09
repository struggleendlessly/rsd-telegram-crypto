using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using Shared.BaseScan;
using Shared.ConfigurationOptions;
using Shared.DB;
using Shared.HealthCheck;
using Shared.Telegram;

using WorkerServiceRead;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSystemd();
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<StripeOptions>(builder.Configuration.GetSection(StripeOptions.SectionName));
builder.Services.Configure<OptionsTelegram>(builder.Configuration.GetSection(OptionsTelegram.SectionName));
builder.Services.Configure<OptionsBaseScan>(builder.Configuration.GetSection(OptionsBaseScan.SectionName));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);

builder.Services.AddMemoryCache();
builder.Services.AddTransient<Telegram>();
builder.Services.AddSingleton<HealthCheck>();
builder.Services.AddSingleton<BaseScanApiClient>();
builder.Services.AddSingleton<BaseScanContractScraper>();

var host = builder.Build();
host.Run();
