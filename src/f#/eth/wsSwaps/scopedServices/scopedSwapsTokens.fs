module scopedSwapsTokens

open System
open System.Threading
open System.Threading.Tasks

open Microsoft.Extensions.Logging

open IScopedProcessingService

open alchemy

type scopedSwapsTokens(
        logger: ILogger<scopedSwapsTokens>,
        alchemy: alchemy) =
    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                

            }
