using api_alchemy.Eth;

using Data;

using eth_shared;

using Microsoft.Extensions.DependencyInjection;

namespace ws_eth_dev
{
    public class Worker : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        private readonly ILogger<Worker> _logger;

        public Worker(
             ILogger<Worker> logger,
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
                GetRequiredKeyedService<IScopedProcessingService>("WorkerDevScoped");
                //GetRequiredKeyedService<IScopedProcessingService>("WorkerScoped");
                //GetRequiredKeyedService<IScopedProcessingService>("Worker2Scoped");
                //GetRequiredKeyedService<IScopedProcessingService>("Worker1Scoped");
                //GetRequiredKeyedService<IScopedProcessingService>("Worker5MinisScoped");

                await scopedProcessingService.DoWorkAsync(stoppingToken);
            }
        }
    }
}
