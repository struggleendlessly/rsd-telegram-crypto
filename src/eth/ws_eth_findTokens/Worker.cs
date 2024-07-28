using Data;

using eth_shared;

using Microsoft.EntityFrameworkCore;

using System;

namespace ws_eth_findTokens
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly FindTransactionService findTransactionService;
        private readonly dbContext dbContext;

        public Worker(
            ILogger<Worker> logger,
            dbContext dbContext,
            FindTransactionService findTransactionService)
        {
            _logger = logger;
            this.dbContext = dbContext;
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

                _logger.LogInformation("Worker processed blocks: {block}", await dbContext.EthBlock.CountAsync());
                _logger.LogInformation("Worker running time: {time}", (timeEnd - timeStart).TotalSeconds);

                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}
