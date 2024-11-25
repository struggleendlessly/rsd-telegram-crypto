module MyScopedProcessingService

open System
open System.Threading
open System.Threading.Tasks

open Microsoft.Extensions.Logging

open IScopedProcessingService

open alchemy

type MyScopedProcessingService(
        logger: ILogger<MyScopedProcessingService>,
        alchemy: alchemy) =
    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let numbers = seq { 21252610 .. 21252620 } |> Seq.toArray

                //alchemy.ShuffleApiKeys()
                let e = alchemy.readData
                let blocks = alchemy.getBlockByNumber numbers 
                let lastBlock = alchemy.getLastBlockNumber() 

                do! Task.Delay(1000)
            }
