using Data;

using eth_shared;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ws_eth_findTokens.ScopedService
{
    public sealed class WorkerScoped : IScopedProcessingService
    {
        private readonly ILogger _logger;
        private readonly Step1 step1;
        private readonly Step2 step2;
        private readonly dbContext dbContext;

        private int _executionCount;

        public WorkerScoped(
            IServiceScopeFactory serviceScopeFactory,
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

        public async Task DoWorkAsync(CancellationToken stoppingToken)
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

                await Task.Delay(35000, stoppingToken);
            }
        }
    }
}
