module scopedSwapsTokens

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open IScopedProcessingService
open Extensions
open responseSwap
open scopedTokenInfo
open scopedSwapsETH
open ethExcludeTokens

open alchemy
open dbMigration
open dbMigration.models

type scopedSwapsTokens(
        logger: ILogger<scopedSwapsTokens>,
        alchemy: alchemy,
        scopedTokenInfo: scopedTokenInfo,
        scopedSwapsETH: scopedSwapsETH,
        ethDB: ethDB) =

    let getLastKnownProcessedBlock () =
        let noBlock = EthSwapsETH_Token.Default()
        let getNumberInt (x: EthSwapsETH_Token) = x.blockNumberEndInt

        ethDB.EthSwapsETH_TokenEntities
             .OrderByDescending(fun x -> x.blockNumberEndInt)
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
              let a = blocks |> Array.copy
              let d =  a |> Array.collect (fun x -> x.result)

              let filtered = blocks |> Array.filter (fun x -> not (Array.isEmpty x.result))

              if Array.isEmpty filtered
              then
                     [| blocks |> Array.maxBy (fun x -> x.id) |]
               else
                     filtered

    let saveToDB blocks = 
        async {
            if Array.isEmpty blocks then
                return 0
            else
                do! ethDB.EthSwapsETH_TokenEntities.AddRangeAsync(blocks) |> Async.AwaitTask
                let! result = ethDB.SaveChangesAsync() |> Async.AwaitTask
                return result
        }

    let getDistinctPairAddresses (responseSwaps: responseSwap[][]) =
        responseSwaps
        |> Array.collect id 
        |> Array.collect (fun swap -> swap.result) 
        |> Array.map (fun res -> res.address) 
     //   |> Array.filter (fun address -> not (tokensExclude |> Array.contains address))
        |> Array.distinct

    let processBlocks (blocks: responseSwap array array) = 
        async {              
                let tokenAddresses =
                    blocks
                    |> getDistinctPairAddresses
                    |> scopedTokenInfo.getToken0and1

                let! decimals =
                    blocks
                    |> getDistinctPairAddresses
                    |> scopedTokenInfo.getDecimals

                let min = blocks[0] |> Array.minBy (fun x -> x.id)
                let max = blocks[0] |> Array.maxBy (fun x -> x.id)
                let! price =
                      blocks[0]
                      |> Array.map (fun x -> float x.id) 
                      |> Array.average
                      |> int
                      |> scopedSwapsETH.getPriceForBlock min.id max.id

                let t =
                        blocks[0]
                        |> Array.map (fun block -> 
                            block.result
                            |> Array.groupBy (fun x -> x.address)    
                            |> Array.filter (fun  (add, res) -> not (tokensExclude |> Array.contains add))
                            //|> Array.Parallel.map(fun (add, res) -> 
                            //    (add, res)
                            //    |> mapResponseSwap.mapResponseSwapResult block.id decimals.[add] price
                            //    )
                            |> Array.Parallel.choose (fun (add, res) -> 
                                match Map.tryFind add decimals with
                                | Some decimalValue -> Some (mapResponseSwap.mapResponseSwapResult block.id tokenAddresses decimalValue price (add, res))
                                | None -> None
                              ) )
                        |> Array.collect id

                return t
        }
    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let t1 = 
                        (getSeqToProcess 120 ethStrings.ethChainBlocksIn5Minutes)

                        |> Async.RunSynchronously 

                let t = 
                        (getSeqToProcess 30 ethStrings.ethChainBlocksIn5Minutes)
                        |> Async.Bind alchemy.getBlockSwapsETH_Tokens  
                        |> Async.Bind processBlocks
                        |> Async.Bind saveToDB
                        |> Async.RunSynchronously 
                return ()

            }
