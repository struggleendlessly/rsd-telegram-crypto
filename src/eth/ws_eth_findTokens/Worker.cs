using api_alchemy.Eth;

using eth_shared;

namespace ws_eth_findTokens
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly FindTransactionService findTransactionService;

        public Worker(
            ILogger<Worker> logger, 
            FindTransactionService findTransactionService)
        {
            _logger = logger;
            this.findTransactionService = findTransactionService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await findTransactionService.Start();

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
