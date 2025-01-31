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


type trigger_0volumeNperiods(
        logger: ILogger<trigger_0volumeNperiods>, 
        serviceScopeFactory: IServiceScopeFactory) =

    inherit BackgroundService()
    
    let schedule = "0/5 * * * *"; // every 5 min
    let _cron = CronExpression.Parse(schedule);

    override this.ExecuteAsync(stoppingToken: CancellationToken) =
        task {
            while not stoppingToken.IsCancellationRequested do
                let utcNow = DateTime.UtcNow
                let nextUtc = _cron.GetNextOccurrence(utcNow)
                let delay = nextUtc.Value - utcNow

                let isTimersOff = String.Equals("true", Environment.GetEnvironmentVariable("DOTNET_TIMERS"), StringComparison.InvariantCultureIgnoreCase)
                if not isTimersOff then
                    do! Task.Delay(delay, stoppingToken) 

                use scope = serviceScopeFactory.CreateScope()
                let serviceFactory = scope.ServiceProvider.GetRequiredService<IDictionary<string, IScopedProcessingService>>()
                let scopedProcessingService = serviceFactory.[scoped_trigger_0volumeNperiods_Name]
                do! scopedProcessingService.DoWorkAsync(stoppingToken) |> Async.AwaitTask |> Async.StartAsTask
        }