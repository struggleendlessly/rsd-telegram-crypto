using eth_shared;

using System;

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
                var timeStart = DateTimeOffset.Now;

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    await findTransactionService.Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Worker Exception: {message}", ex.Message);
                    _logger.LogError("Worker Exception: {stack}", ex.StackTrace);
                }

                var timeEnd = DateTimeOffset.Now;

                _logger.LogInformation("Worker stopped at: {time}", DateTimeOffset.Now);
                _logger.LogInformation("Worker running time: {time}", (timeEnd - timeStart).TotalSeconds);

                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}
