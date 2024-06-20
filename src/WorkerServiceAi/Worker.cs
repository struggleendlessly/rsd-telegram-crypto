using WorkerServiceAi.DB;

namespace WorkerServiceAi
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DBContext dbContext;

        public Worker(ILogger<Worker> logger,
            DBContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var a = dbContext.Learn22.ToList();

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}