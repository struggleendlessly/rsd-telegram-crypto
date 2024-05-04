using Microsoft.EntityFrameworkCore;

using Shared.BaseScan;
using Shared.ConfigurationOptions;
using Shared.DB;
using Shared.Filters;
using Shared.Telegram;

using WorkerServiceCryptoFilter;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSystemd();
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<OptionsBaseScan>(builder.Configuration.GetSection(OptionsBaseScan.SectionName));
builder.Services.Configure<OptionsTelegram>(builder.Configuration.GetSection(OptionsTelegram.SectionName));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);

builder.Services.AddSingleton<Telegram>();
builder.Services.AddSingleton<BaseScan>();
builder.Services.AddSingleton<CryptoFilter>();

var host = builder.Build();
host.Run();
