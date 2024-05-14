using Shared.BaseScan;
using Shared.HealthCheck;

namespace WorkerServiceRead
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly BaseScanContractScraper baseScanContractScraper;
        private readonly HealthCheck healthCheck;

        public Worker(
            ILogger<Worker> logger,
            BaseScanContractScraper baseScanContractScraper,
            HealthCheck healthCheck)
        {
            _logger = logger;
            this.healthCheck = healthCheck;
            this.baseScanContractScraper = baseScanContractScraper;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                await healthCheck.StartWithInfo("Reader");

                try
                {
                    await baseScanContractScraper.Start();
                }
                catch (Exception ex)
                {

                }

                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
