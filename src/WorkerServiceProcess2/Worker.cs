using Shared.Filters;
using Shared.HealthCheck;

namespace WorkerServiceProcess2
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CryptoFilterProcess2 CryptoFilterProcess2;
        private readonly HealthCheck healthCheck;
        public Worker(
            ILogger<Worker> logger,
            CryptoFilterProcess2 cryptoFilter,
            HealthCheck healthCheck)
        {
            _logger = logger;
            this.CryptoFilterProcess2 = cryptoFilter;
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
                    await healthCheck.StartNoInfo("Process2", true);
                    await CryptoFilterProcess2.Start();
                }
                catch (Exception ex)
                {

                }

                await Task.Delay(1800000, stoppingToken);
            }
        }
    }
}
