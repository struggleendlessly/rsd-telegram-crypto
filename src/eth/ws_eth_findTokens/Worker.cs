using eth_shared;

namespace ws_eth_findTokens
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory serviceScopeFactory;

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
                    GetRequiredKeyedService<IScopedProcessingService>("WorkerScoped");

                await scopedProcessingService.DoWorkAsync(stoppingToken);
            }
        }
    }
}
