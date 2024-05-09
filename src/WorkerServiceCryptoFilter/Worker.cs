using Shared.Filters;
using Shared.HealthCheck;

namespace WorkerServiceCryptoFilter
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CryptoFilter cryptoFilter;
        private readonly HealthCheck healthCheck;
        public Worker(
            ILogger<Worker> logger,
            CryptoFilter cryptoFilter,
            HealthCheck healthCheck)
        {
            _logger = logger;
            this.cryptoFilter = cryptoFilter;
            this.healthCheck = healthCheck;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                await healthCheck.Start("CryptoFilter");
                await cryptoFilter.Start();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
