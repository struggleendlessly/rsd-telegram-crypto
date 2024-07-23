using api_alchemy.Eth;

namespace ws_eth_findTokens
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly EthApi apiAlchemy;

        public Worker(ILogger<Worker> logger, EthApi apiAlchemy)
        {
            _logger = logger;
            this.apiAlchemy = apiAlchemy;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var aa = await apiAlchemy.getBlockByNumber(20368087);

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
