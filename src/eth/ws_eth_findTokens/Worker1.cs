using Data;

using eth_shared;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using ws_eth_findTokens.ScopedService;

namespace ws_eth_findTokens
{
    public class Worker1 : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public Worker1(
            ILogger<Worker1> logger,
            IServiceScopeFactory serviceScopeFactory
            )
        {
            _logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                IScopedProcessingService scopedProcessingService =
                    scope.
                    ServiceProvider.
                    GetRequiredKeyedService<IScopedProcessingService>("2");

                await scopedProcessingService.DoWorkAsync(stoppingToken);
            }
        }
    }
}
