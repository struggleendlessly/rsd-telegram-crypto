module scopedSwapsETH

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open IScopedProcessingService
open Extensions
open responseSwap

open alchemy
open ethCommonDB
open ethCommonDB.models
open createSeq
open Microsoft.Extensions.Options
open ChainSettingsOptionModule

type scopedSwapsETH(
        logger: ILogger<scopedSwapsETH>,
        alchemy: alchemy,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;

    let getLastKnownProcessedBlock () =
        let noBlock = SwapsETH_USD.Default(chainSettingsOption.DefaultBlockNumber)
        let getNumberInt (x: SwapsETH_USD) = x.blockNumberInt

        ethDB.EthSwapsETH_USDEntities
             .OrderByDescending(fun x -> x.blockNumberInt)
             .FirstOrDefaultAsync()
             |> Async.AwaitTask
             |> Async.map (Option.ofObj 
                                >> Option.defaultValue noBlock
                                >> getNumberInt)

    let getLastEthBlock () =
        let noBlock = BlocksEntity.Default(chainSettingsOption.DefaultBlockNumber)
        let getNumberInt (x: BlocksEntity) = x.numberInt

        ethDB.EthBlocksEntities
                .OrderByDescending(fun x -> x.numberInt)
                .FirstOrDefaultAsync()
                |> Async.AwaitTask
                |> Async.map (Option.ofObj 
                                >> Option.defaultValue noBlock
                                >> getNumberInt)

    let saveToDB blocks = 
        async {
            if Array.isEmpty blocks then
                return 0
            else
                do! ethDB.EthSwapsETH_USDEntities.AddRangeAsync(blocks) |> Async.AwaitTask
                let! result = ethDB.SaveChangesAsync() |> Async.AwaitTask
                return result
        }  
        
    let processBlocks =                    
        Array.collect id
        >> validateBlocks
        >> Array.Parallel.map(fun block -> 
            block 
            |> mapResponseSwap.mapETH_USD chainSettingsOption.AddressChainCoinDecimals chainSettingsOption.AddressChainCoin chainSettingsOption.AddressStableCoin
        )
        
    let getDefaultPrice() =
        ethDB.EthSwapsETH_USDEntities
            .OrderByDescending(fun x -> x.blockNumberInt)
            .FirstOrDefaultAsync()                   
            |> Async.AwaitTask

    member this.getPriceForBlock min max blockInt =
        async {
            let! a =
                ethDB.EthSwapsETH_USDEntities
                    .Where(fun block -> block.blockNumberInt >= min && block.blockNumberInt <= max)
                    .Take(10)
                    .ToListAsync()
                |> Async.AwaitTask

            if a.Count = 0 then
                let! defaultPrice = getDefaultPrice()
                return defaultPrice.priceEthInUsd
            else
                return  a
                        |> Seq.minBy (fun x -> Math.Abs(x.blockNumberInt - blockInt))
                        |> fun x -> x.priceEthInUsd
        }        

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let t = 
                        (getSeqToProcess 1000 chainSettingsOption.BlocksIn5Minutes getLastKnownProcessedBlock getLastEthBlock)
                        |> Async.Bind alchemy.getBlockSwapsETH_USD
                        |> Async.map processBlocks
                        |> Async.Bind saveToDB
                        |> Async.RunSynchronously                      

                return ()
            }
