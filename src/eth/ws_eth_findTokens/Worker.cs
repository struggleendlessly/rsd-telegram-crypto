using api_alchemy;

namespace ws_eth_findTokens
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ApiAlchemy apiAlchemy;

        public Worker(ILogger<Worker> logger, ApiAlchemy apiAlchemy)
        {
            _logger = logger;
            this.apiAlchemy = apiAlchemy;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var aa = await apiAlchemy.getBlockByNumber(1);

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
