using Microsoft.EntityFrameworkCore;

using Shared.ConfigurationOptions;
using Shared.DB;
using Shared.Telegram;

using WorkerServiceReadTelegram;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSystemd();
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<StripeOptions>(builder.Configuration.GetSection(StripeOptions.SectionName));
builder.Services.Configure<OptionsTelegram>(builder.Configuration.GetSection(OptionsTelegram.SectionName));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);

builder.Services.AddSingleton<Telegram>();

var host = builder.Build();
host.Run();
