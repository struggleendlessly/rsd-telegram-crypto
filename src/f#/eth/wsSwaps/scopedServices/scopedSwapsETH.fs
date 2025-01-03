module scopedSwapsETH

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore
open System.Collections.Generic
open IScopedProcessingService
open Extensions
open responseSwap

open alchemy
open dbMigration
open dbMigration.models

type scopedSwapsETH(
        logger: ILogger<scopedSwapsETH>,
        alchemy: alchemy,
        ethDB: ethDB) =

    let getLastKnownProcessedBlock () =
        let noBlock = EthSwapsETH_USD.Default(21545152)
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

    let getSeqToProcess n step =
        async{
            let! startAsync  = getLastKnownProcessedBlock()
            let! endAsync = getLastEthBlock()
            
            if endAsync - startAsync > n
            then
                return seq { startAsync + 1 .. step .. startAsync + n } |> Seq.toArray
            elif endAsync - startAsync > step
            then
                return seq { startAsync + 1 .. step .. endAsync } |> Seq.toArray
            else
                return [||]
        }

    let filterBlocks (blocks:responseSwap[]) = 
              let filtered = blocks |> Array.filter (fun x -> not (Array.isEmpty x.result))
             
              if Array.isEmpty blocks 
              then
                     blocks             
              elif Array.isEmpty filtered
              then
                     [| blocks |> Array.maxBy (fun x -> x.id) |]
               else
                     filtered

    let saveToDB blocks = 
        async {
            if Array.isEmpty blocks then
                return 0
            else
                do! ethDB.EthSwapsETH_USDEntities.AddRangeAsync(blocks) |> Async.AwaitTask
                let! result = ethDB.SaveChangesAsync() |> Async.AwaitTask
                return result
        }  
        
    let processBlocks blocks = 
        async {              
               return 
                        blocks
                        |> Array.collect id
                        |> filterBlocks
                        |> Array.Parallel.map(fun block -> 
                            block 
                            |> mapResponseSwap.map
                        )
        }

    let createGenericList (values: 'T seq) : List<'T> = List<'T>(values)
    let getDefaultPrice() =

            let a = 
                   ethDB.EthSwapsETH_USDEntities
                    .OrderByDescending(fun x -> x.blockNumberInt)
                    .FirstOrDefaultAsync()                   
                    |> Async.AwaitTask
            a

    member this.getPriceForBlock min max (blockInt: int) =
        async {
            let! a =
                ethDB.EthSwapsETH_USDEntities
                    .Where(fun block -> block.blockNumberInt >= min && block.blockNumberInt <= max)
                    .ToListAsync()
                |> Async.AwaitTask

            if a.Count = 0 then
                let! defaultPrice = getDefaultPrice()
                return defaultPrice.priceEthInUsd
            else
                let e = a
                        |> List.ofSeq
                        |> List.minBy (fun x -> Math.Abs(x.blockNumberInt - blockInt))
                return e.priceEthInUsd
        }        

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let t = 
                        (getSeqToProcess 100 ethStrings.ethChainBlocksIn5Minutes)
                        |> Async.Bind (alchemy.getBlockSwapsETH_USD ethStrings.addressDai ethStrings.topicSwap ethStrings.ethChainBlocksIn5Minutes )
                        |> Async.Bind processBlocks
                        |> Async.Bind saveToDB
                        |> Async.RunSynchronously                      

                return ()
            }
