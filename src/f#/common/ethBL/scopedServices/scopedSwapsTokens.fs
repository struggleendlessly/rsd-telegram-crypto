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
                let a = blocks |> Array.map (fun (x: SwapsETH_Token) -> x.priceTokenInETH)
                ethDB.swapsETH_TokenEntities.AddRangeAsync(blocks) |> Async.AwaitTask |> ignore
                let result = ethDB.SaveChangesAsync() |> Async.AwaitTask
                result
        
    let getDistinctPairAddresses =
        Seq.collect id 
        >> Seq.collect (fun swap -> swap.result) 
        >> Seq.map (fun res -> res.address) 
        >> Seq.filter (fun address -> not (chainSettingsOption.ExcludedAddresses |> Seq.contains address))
        >> Seq.distinct
    
    let processBlocks (blocks: responseSwap seq seq) = 
        async {       
                if Seq.isEmpty blocks 
                then
                    let emptyArray : SwapsETH_Token seq = Seq.empty
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
            
                    let min = blocks |> Seq.head |> Seq.minBy (fun x -> x.id) 
                    let max = blocks |> Seq.head |> Seq.maxBy (fun x -> x.id)
                    let! priceAverageForBlocks = scopedSwapsETH.getPriceForBlock (min.id - 100) (max.id + 100)

                    let t =
                            blocks
                            |> Seq.head
                            |> Seq.map (fun block -> 
                                block.result
                                |> Array.groupBy (fun x -> x.address)    
                                |> Array.filter (fun  (add, res) -> not (chainSettingsOption.ExcludedAddresses |> Array.contains add))
                                |> Array.choose (fun (add, res) -> 
                                    match Map.tryFind add decimals with
                                    | Some decimalValue -> Some (mapResponseSwap.mapResponseSwapResult 
                                                                        chainSettingsOption.AddressChainCoin 
                                                                        chainSettingsOption.AddressStableCoinsToInteract
                                                                        chainSettingsOption.BlocksIn5Minutes 
                                                                        block.id 
                                                                        tokenAddresses 
                                                                        decimalValue 
                                                                        priceAverageForBlocks 
                                                                        (add, res))
                                    | None -> None
                                                   ) 
                                    |> Seq.choose id
                                  )                           
                            |> Seq.collect id

                    return t
        }
    interface IScopedProcessingService with

        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                do! 
                        (getSeqToProcess 
                                (chainSettingsOption.BlocksIn5Minutes * 6) 
                                chainSettingsOption.BlocksIn5Minutes 
                                getLastKnownProcessedBlock 
                                getLastEthBlock)
                        |> Async.Bind (alchemy.getBlockSwapsETH_Tokens)
                        |> Async.Bind processBlocks
                        |> Async.map (Seq.toArray >> saveToDB)
                        |> Async.Ignore 

            }
