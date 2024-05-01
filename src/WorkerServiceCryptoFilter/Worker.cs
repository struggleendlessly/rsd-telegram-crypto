using Shared.Filters;

namespace WorkerServiceCryptoFilter
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CryptoFilter cryptoFilter;
        public Worker(ILogger<Worker> logger, CryptoFilter cryptoFilter)
        {
            _logger = logger;
            this.cryptoFilter = cryptoFilter;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                await cryptoFilter.Start();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
