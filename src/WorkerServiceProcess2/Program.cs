using Microsoft.EntityFrameworkCore;

using Shared.BaseScan;
using Shared.ConfigurationOptions;
using Shared.DB;
using Shared.Filters;
using Shared.HealthCheck;
using Shared.Telegram;

using WorkerServiceProcess2;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSystemd();
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<OptionsBanAddresses>(builder.Configuration.GetSection(OptionsBanAddresses.SectionName));
builder.Services.Configure<OptionsBaseScan>(builder.Configuration.GetSection(OptionsBaseScan.SectionName));
builder.Services.Configure<OptionsTelegram>(builder.Configuration.GetSection(OptionsTelegram.SectionName));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);

builder.Services.AddMemoryCache();
builder.Services.AddTransient<Telegram>();
builder.Services.AddTransient<HealthCheck>();
builder.Services.AddTransient<CryptoFilterProcess2>();
builder.Services.AddTransient<BaseScanApiClient>();

var host = builder.Build();
host.Run();
