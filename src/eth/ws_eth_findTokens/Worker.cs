using Data;

using eth_shared;

using Microsoft.EntityFrameworkCore;

namespace ws_eth_findTokens
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Step1 step1;
        private readonly Step2 step2;
        private readonly dbContext dbContext;

        public Worker(
            ILogger<Worker> logger,
            dbContext dbContext,
            Step1 step1,
            Step2 step2
            )
        {
            _logger = logger;
            this.dbContext = dbContext;
            this.step1 = step1;
            this.step2 = step2;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var timeStartStep1 = DateTimeOffset.Now;

                _logger.LogInformation("Worker step1 running at: {time}", DateTimeOffset.Now);

                try
                {
                    await step1.Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Worker step1 Exception: {message}", ex.Message);
                    _logger.LogError("Worker step1 Exception: {stack}", ex.StackTrace);
                }

                var timeEndStep1 = DateTimeOffset.Now;

                _logger.LogInformation("Worker step1 processed blocks: {block}", await dbContext.EthBlock.CountAsync());
                _logger.LogInformation("Worker step1 running time: {time}", (timeEndStep1 - timeStartStep1).TotalSeconds);

                var timeStartStep2 = DateTimeOffset.Now;
                _logger.LogInformation("Worker step2 running at: {time}", DateTimeOffset.Now);

                try
                {
                    await step2.Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Worker step2 Exception: {message}", ex.Message);
                    _logger.LogError("Worker step2 Exception: {stack}", ex.StackTrace);
                }

                var timeEndStep2 = DateTimeOffset.Now;

                _logger.LogInformation("Worker step2 running time: {time}", (timeEndStep2 - timeStartStep2).TotalSeconds);

                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}
