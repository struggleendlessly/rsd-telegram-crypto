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
open bl_createSeq
open Microsoft.Extensions.Options
open ChainSettingsOptionModule

type scopedSwapsETH(
        logger: ILogger<scopedSwapsETH>,
        alchemy: alchemyEVM,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;

    let getLastKnownProcessedBlock () =
        let noBlock = SwapsETH_USD.Default(int chainSettingsOption.DefaultBlockNumber)
        let getNumberInt (x: SwapsETH_USD) = x.blockNumberInt

        ethDB.swapsETH_USDEntities
             .OrderByDescending(fun x -> x.blockNumberInt)
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
        async {
            if Seq.isEmpty blocks then
                return 0
            else
                do! ethDB.swapsETH_USDEntities.AddRangeAsync(blocks) |> Async.AwaitTask
                let! result = ethDB.SaveChangesAsync() |> Async.AwaitTask
                return result
        }  
        
    let processBlocks =                    
        Seq.collect id
        >> validateBlocks
        >> Seq.map(fun block -> 
            block 
            |> mapResponseSwap.mapETH_USD 
                    chainSettingsOption.AddressChainCoin 
                    chainSettingsOption.AddressChainCoinDecimals 
                    chainSettingsOption.AddressChainCoin 
                    chainSettingsOption.AddressStableCoin
        )
        
    let getDefaultPrice() =
        ethDB.swapsETH_USDEntities
            .OrderByDescending(fun x -> x.blockNumberInt)
            .FirstOrDefaultAsync()                   
            |> Async.AwaitTask

    member this.getPriceForBlock min max =
        async {
            let blockIntMidle = (min + max) / 2
            let! a =
                ethDB.swapsETH_USDEntities
                    .Where(fun block -> block.blockNumberInt >= min && block.blockNumberInt <= max)
                    .Take(10)
                    .ToListAsync()
                |> Async.AwaitTask

            if a.Count = 0 then
                let! defaultPrice = getDefaultPrice()
                return defaultPrice.priceEthInUsd
            else
                return  a
                        |> Seq.minBy (fun x -> Math.Abs(x.blockNumberInt - blockIntMidle))
                        |> fun x -> x.priceEthInUsd
        }        

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                do! 
                        (getSeqToProcess 1000 chainSettingsOption.BlocksIn5Minutes getLastKnownProcessedBlock getLastEthBlock)
                        |> Async.Bind alchemy.getBlockSwapsETH_USD
                        |> Async.map (processBlocks >> Seq.toArray) 
                        |> Async.Bind saveToDB
                        |> Async.Ignore                    

            }
