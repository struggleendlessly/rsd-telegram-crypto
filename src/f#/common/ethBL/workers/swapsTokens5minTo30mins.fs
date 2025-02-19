namespace workers

open System
open System.Threading
open System.Threading.Tasks
open System.Collections.Generic

open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection

open IScopedProcessingService
open scopedNames
open Cronos
open Microsoft.Extensions.Options
open debugSettingsOption

type swapsTokens5minTo30mins(
        logger: ILogger<tokenInfo>, 
        debugSettingsOption: IOptions<debugSettingsOption>, 
        serviceScopeFactory: IServiceScopeFactory) =

    inherit BackgroundService()
    
    let debugSettings = debugSettingsOption.Value;
    let schedule = "0/1 * * * *"; // every 5 min
    let _cron = CronExpression.Parse(schedule);

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        task {
            while not stoppingToken.IsCancellationRequested do

                let utcNow = DateTime.UtcNow
                let nextUtc = _cron.GetNextOccurrence(utcNow)
                let delay = nextUtc.Value - utcNow

                if debugSettings.delayOnOff = 1 then
                    do! Task.Delay(delay, stoppingToken) 

                use scope = serviceScopeFactory.CreateScope()
                let serviceFactory = scope.ServiceProvider.GetRequiredService<IDictionary<string, IScopedProcessingService>>()
                let scopedProcessingService = serviceFactory.[scoped_swapsTokens5minTo30mins_Name]
                do! scopedProcessingService.DoWorkAsync(stoppingToken)(0)
        }