module scopedSwapsETH

open System
open System.Threading
open System.Threading.Tasks

open Microsoft.Extensions.Logging

open IScopedProcessingService

open alchemy

type scopedSwapsETH(
        logger: ILogger<scopedSwapsETH>,
        alchemy: alchemy) =
    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let numbers = seq { 21252610 .. 21252620 } |> Seq.toArray

                alchemy.ShuffleApiKeys()
                let blocks = alchemy.getBlockByNumber numbers 
                let lastBlock = alchemy.getLastBlockNumber() 

                logger.LogInformation("Last block: {lastBlock}", lastBlock)
            }
