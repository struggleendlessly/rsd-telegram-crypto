using eth_shared;

namespace ws_eth_5mins
{
    public class Worker4 : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public Worker4(
            ILogger<Worker4> logger,
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
                    GetRequiredKeyedService<IScopedProcessingService>("Worker4Scoped");

                await scopedProcessingService.DoWorkAsync(stoppingToken);
            }
        }
    }
}
