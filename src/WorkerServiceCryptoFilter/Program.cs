using Microsoft.EntityFrameworkCore;

using Shared.DB;
using Shared.Filters;

using WorkerServiceCryptoFilter;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSystemd();
builder.Services.AddHostedService<Worker>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);

builder.Services.AddSingleton<CryptoFilter>();

var host = builder.Build();
host.Run();
