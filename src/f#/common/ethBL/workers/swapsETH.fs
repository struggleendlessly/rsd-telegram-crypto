namespace workers

open System.Threading
open System.Threading.Tasks
open System.Collections.Generic

open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

open IScopedProcessingService
open scopedNames

type swapsETH(
        logger: ILogger<swapsETH>, 
        serviceScopeFactory: IServiceScopeFactory) =
    inherit BackgroundService()

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        task {
            while not stoppingToken.IsCancellationRequested do
                logger.LogInformation("Worker swapsETH running at")
                use scope = serviceScopeFactory.CreateScope()
                let serviceFactory = scope.ServiceProvider.GetRequiredService<IDictionary<string, IScopedProcessingService>>()
                let scopedProcessingService = serviceFactory.[scopedSwapsETHName]
                
                try
                    do! scopedProcessingService.DoWorkAsync(stoppingToken) 0
                with ex ->
                    logger.LogError(ex, "Error in swapsETH: {message}", ex.Message)

                do! Task.Delay(12_000, stoppingToken)
        }