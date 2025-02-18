namespace workers

open System.Threading
open System.Threading.Tasks
open System.Collections.Generic

open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

open IScopedProcessingService
open scopedNames
open Microsoft.Extensions.Options
open debugSettingsOption

type swapsTokens(
        logger: ILogger<swapsTokens>, 
        debugSettingsOption: IOptions<debugSettingsOption>, 
        serviceScopeFactory: IServiceScopeFactory) =
    inherit BackgroundService()

    let debugSettings = debugSettingsOption.Value;

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        task {
            while not stoppingToken.IsCancellationRequested do                
                logger.LogInformation("Worker swapsTokens running at")
                use scope = serviceScopeFactory.CreateScope()
                let serviceFactory = scope.ServiceProvider.GetRequiredService<IDictionary<string, IScopedProcessingService>>()
                let scopedProcessingService = serviceFactory.[scopedSwapsTokensName]

                try
                    do! scopedProcessingService.DoWorkAsync(stoppingToken)(0)
                with 
                | ex -> logger.LogError(ex, "Error in swapsTokens")

                do! Task.Delay(60_000, stoppingToken)
        }