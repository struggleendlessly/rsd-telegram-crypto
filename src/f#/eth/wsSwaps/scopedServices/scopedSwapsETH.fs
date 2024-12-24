module scopedSwapsETH

open System
open System.Threading
open System.Threading.Tasks
open System.Linq

open Microsoft.Extensions.Logging

open IScopedProcessingService
open dbMigration
open dbMigration.models
open alchemy
open Extensions
open Microsoft.EntityFrameworkCore
open responseGetBlock
open responseGetLastBlock

type scopedSwapsETH(
        logger: ILogger<scopedSwapsETH>,
        alchemy: alchemy,
        ethDB: ethDB) =

    let getLastKnownProcessedBlock () =
        let noBlock = EthSwapsETH_USD.Default()
        let getNumberInt (x: EthSwapsETH_USD) = x.blockNumberInt

        ethDB.EthSwapsETH_USDEntities
             .OrderByDescending(fun x -> x.blockNumberInt)
             .FirstOrDefaultAsync()
             |> Async.AwaitTask
             |> Async.map (Option.ofObj 
                                >> Option.defaultValue noBlock
                                >> getNumberInt)

    let getLastEthBlock () =
        let noBlock = EthBlocksEntity.Default()
        let getNumberInt (x: EthBlocksEntity) = x.numberInt

        ethDB.EthBlocksEntities
                .OrderByDescending(fun x -> x.numberInt)
                .FirstOrDefaultAsync()
                |> Async.AwaitTask
                |> Async.map (Option.ofObj 
                                >> Option.defaultValue noBlock
                                >> getNumberInt)

    let getSeqToProcess n =
        async{
            let! startAsync  = getLastKnownProcessedBlock()
            let! endAsync = getLastEthBlock()
            
            if endAsync - startAsync > n
            then
                return seq { startAsync + 1 .. startAsync + n } |> Seq.toArray
            else 
                return seq { startAsync + 1 .. endAsync } |> Seq.toArray
        }


    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let t = 
                        (getSeqToProcess 100)
                        |> Async.Bind alchemy.getBlockSwapsETH_USD  
                        |> Async.RunSynchronously

                return ()
            }
