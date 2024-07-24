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
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                await findTransactionService.Start();

                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}
