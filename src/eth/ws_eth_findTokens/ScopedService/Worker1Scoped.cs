using Data;

using eth_shared;

using Microsoft.EntityFrameworkCore;

namespace ws_eth_findTokens.ScopedService
{
    public sealed class Worker1Scoped : IScopedProcessingService
    {
        private readonly ILogger _logger;
        private readonly Step1 step1;
        private readonly Step3 step3;
        private readonly dbContext dbContext;

        private int _executionCount;

        public Worker1Scoped(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<Worker> logger,
            dbContext dbContext,
            Step1 step1,
            Step3 step3
            )
        {
            _logger = logger;
            this.dbContext = dbContext;
            this.step1 = step1;
            this.step3 = step3;
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var timeStartStep1 = DateTimeOffset.Now;

                _logger.LogInformation("Worker step3 running at: {time}", DateTimeOffset.Now);

                try
                {
                     await step3.Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Worker step3 Exception: {message}", ex.Message);
                    _logger.LogError("Worker step3 Exception: {stack}", ex.StackTrace);
                }

                var timeEndStep1 = DateTimeOffset.Now;

                _logger.LogInformation("Worker step3 processed blocks: {block}", await dbContext.EthBlock.CountAsync());

                await Task.Delay(300000, stoppingToken);
            }
        }
    }
}
