using Shared.BaseScan;
using Shared.Filters;
using Shared.HealthCheck;

namespace WorkerServiceCryptoFilter
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CryptoFilterProcess1 CryptoFilterProcess1;
        private readonly HealthCheck healthCheck;
        public Worker(
            ILogger<Worker> logger,
            CryptoFilterProcess1 cryptoFilter,
            HealthCheck healthCheck)
        {
            _logger = logger;
            this.CryptoFilterProcess1 = cryptoFilter;
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


                try
                {
                    await healthCheck.StartNoInfo("CryptoFilter");
                    await CryptoFilterProcess1.Start();
                }
                catch (Exception ex)
                {

                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
