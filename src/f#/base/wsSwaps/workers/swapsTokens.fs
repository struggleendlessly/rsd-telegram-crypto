namespace wsSwaps

open System.Threading
open System.Threading.Tasks
open System.Collections.Generic

open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

open IScopedProcessingService
open scopedNames

type swapsTokens(
        logger: ILogger<swapsTokens>, 
        serviceScopeFactory: IServiceScopeFactory) =
    inherit BackgroundService()

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        task {
            while not stoppingToken.IsCancellationRequested do
                use scope = serviceScopeFactory.CreateScope()
                let serviceFactory = scope.ServiceProvider.GetRequiredService<IDictionary<string, IScopedProcessingService>>()
                let scopedProcessingService = serviceFactory.[scopedSwapsTokens]
                do! scopedProcessingService.DoWorkAsync(stoppingToken)
                do! Task.Delay(1000, stoppingToken)
        }