using Microsoft.EntityFrameworkCore;

using WorkerServiceAi;
using WorkerServiceAi.DB;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);

builder.Services.AddHostedService<Worker>();
builder.Services.AddTransient<ClassificationBaseScan>();

var host = builder.Build();
host.Run();
