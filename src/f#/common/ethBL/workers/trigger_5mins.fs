namespace workers

open System.Threading
open System.Threading.Tasks
open System.Collections.Generic

open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

open IScopedProcessingService
open scopedNames

type trigger_5mins(
        logger: ILogger<trigger_5mins>, 
        serviceScopeFactory: IServiceScopeFactory) =
    inherit BackgroundService()

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        task {
            while not stoppingToken.IsCancellationRequested do
                use scope = serviceScopeFactory.CreateScope()
                let serviceFactory = scope.ServiceProvider.GetRequiredService<IDictionary<string, IScopedProcessingService>>()
                let scopedProcessingService = serviceFactory.[scoped_trigger_5mins_Name]
                do! scopedProcessingService.DoWorkAsync(stoppingToken)
                do! Task.Delay(300_000, stoppingToken)
        }