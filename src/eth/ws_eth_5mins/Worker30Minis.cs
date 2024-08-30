using eth_shared;

namespace ws_eth_5mins
{
    public class Worker30Minis : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public Worker30Minis(
            ILogger<Worker30Minis> logger,
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
                    GetRequiredKeyedService<IScopedProcessingService>("Worker30MinisScoped");

                await scopedProcessingService.DoWorkAsync(stoppingToken);
            }
        }
    }
}
