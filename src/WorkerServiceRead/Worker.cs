using Shared.BaseScan;

namespace WorkerServiceRead
{
    public class Worker : BackgroundService
    {

        private readonly ILogger<Worker> _logger;
        private readonly BaseScanContractScraper baseScanContractScraper;

        public Worker(
            ILogger<Worker> logger, 
            BaseScanContractScraper baseScanContractScraper)
        {
            _logger = logger;
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

                await baseScanContractScraper.Start();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
