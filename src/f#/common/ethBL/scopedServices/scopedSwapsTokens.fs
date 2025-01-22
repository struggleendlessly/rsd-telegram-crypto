module scopedSwapsTokens

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open IScopedProcessingService
open ChainSettingsOptionModule
open Extensions
open responseSwap
open scopedTokenInfo
open scopedSwapsETH
open bl_createSeq

open alchemy
open ethCommonDB
open ethCommonDB.models
open Microsoft.Extensions.Options

type scopedSwapsTokens(
        logger: ILogger<scopedSwapsTokens>,
        alchemy: alchemyEVM,
        scopedTokenInfo: scopedTokenInfo,
        scopedSwapsETH: scopedSwapsETH,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;

    let getLastKnownProcessedBlock () =
        let noBlock = SwapsETH_Token.Default(int chainSettingsOption.DefaultBlockNumber)
        let getNumberInt (x: SwapsETH_Token) = x.blockNumberEndInt

        ethDB.swapsETH_TokenEntities
             .OrderByDescending(fun x -> x.blockNumberEndInt)
             .FirstOrDefaultAsync()
             |> Async.AwaitTask
             |> Async.map (Option.ofObj 
                                >> Option.defaultValue noBlock
                                >> getNumberInt)

    let getLastEthBlock () =
        let noBlock = BlocksEntity.Default(int chainSettingsOption.DefaultBlockNumber)
        let getNumberInt (x: BlocksEntity) = x.numberInt

        ethDB.blocksEntities
                .OrderByDescending(fun x -> x.numberInt)
                .FirstOrDefaultAsync()
                |> Async.AwaitTask
                |> Async.map (Option.ofObj 
                                >> Option.defaultValue noBlock
                                >> getNumberInt)

    let saveToDB blocks = 
                ethDB.swapsETH_TokenEntities.AddRangeAsync(blocks) |> Async.AwaitTask |> ignore
                let result = ethDB.SaveChangesAsync() |> Async.AwaitTask
                result
        
    let getDistinctPairAddresses =
        Array.collect id 
        >> Array.collect (fun swap -> swap.result) 
        >> Array.map (fun res -> res.address) 
        >> Array.filter (fun address -> not (chainSettingsOption.ExcludedAddresses |> Array.contains address))
        >> Array.distinct
    
    let processBlocks (blocks: responseSwap array array) = 
        async {       
                if Array.isEmpty blocks 
                then
                    let emptyArray : SwapsETH_Token array = [||]
                    return emptyArray
                else

                    let distinctPairAddresses =
                        blocks
                        |> getDistinctPairAddresses

                    let! tokenAddresses =
                        distinctPairAddresses
                        |> scopedTokenInfo.getToken0and1

                    let! decimals =
                        distinctPairAddresses
                        |> scopedTokenInfo.getDecimals
            
                    let min = blocks[0] |> Array.minBy (fun x -> x.id) 
                    let max = blocks[0] |> Array.maxBy (fun x -> x.id)
                    let! price =
                          blocks[0]
                          |> Array.map (fun x -> float x.id) 
                          |> Array.average
                          |> int
                          |> scopedSwapsETH.getPriceForBlock (min.id - 100) (max.id + 100)

                    let t =
                            blocks[0]
                            |> Array.map (fun block -> 
                                block.result
                                |> Array.groupBy (fun x -> x.address)    
                                |> Array.filter (fun  (add, res) -> not (chainSettingsOption.ExcludedAddresses |> Array.contains add))
                                |> Array.Parallel.choose (fun (add, res) -> 
                                    match Map.tryFind add decimals with
                                    | Some decimalValue -> Some (mapResponseSwap.mapResponseSwapResult 
                                                                        chainSettingsOption.AddressChainCoin 
                                                                        chainSettingsOption.BlocksIn5Minutes 
                                                                        block.id 
                                                                        tokenAddresses 
                                                                        decimalValue 
                                                                        price 
                                                                        (add, res))
                                    | None -> None
                                  ) )
                            |> Array.collect id

                    return t
        }
    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let t = 
                        (getSeqToProcess 150 chainSettingsOption.BlocksIn5Minutes getLastKnownProcessedBlock getLastEthBlock)
                        |> Async.Bind (alchemy.getBlockSwapsETH_Tokens)
                        |> Async.Bind processBlocks
                        |> Async.map saveToDB
                        |> Async.RunSynchronously 
                return ()

            }
