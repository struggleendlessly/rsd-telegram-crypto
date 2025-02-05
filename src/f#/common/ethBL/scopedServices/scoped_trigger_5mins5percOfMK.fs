module scoped_trigger_5mins5percOfMK

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.Options

open ChainSettingsOptionModule
open IScopedProcessingService
open Extensions
open responseSwap
open scoped_telegram
open bl_others

open alchemy
open ethCommonDB
open ethCommonDB.models
open Nethereum.Util
open System.Net.Http

type scoped_trigger_5mins5percOfMK(
        logger: ILogger<scoped_trigger_5mins5percOfMK>,
        alchemy: alchemyEVM,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        scoped_telegram: scoped_telegram,

        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;

    let getLastProcessedBlock () =
        ethDB.swapsETH_TokenEntities
            .OrderByDescending(fun x -> x.blockNumberEndInt)
            .Take(1)
            .Select(fun x -> x.blockNumberEndInt)
            .FirstOrDefaultAsync()
            |> Async.AwaitTask

    let getLatestTrigger () =
        let noBlock = TriggerHistory.Default()
        let getNumberInt (x: TriggerHistory) = x.blockNumberEndInt

        ethDB.triggerHistoriesEntities
            .Where(fun x -> x.title = scopedNames.trigger_5mins5percOfMK_Name)
            .OrderByDescending(fun x -> x.blockNumberEndInt)
            .Take(1)
            .FirstOrDefaultAsync()
            |> Async.AwaitTask
            |> Async.map (Option.ofObj 
                >> Option.defaultValue noBlock
                >> getNumberInt)

    let getTxnsForPeriod (blockInt) =
        ethDB.swapsETH_TokenEntities
            .Where(fun x -> x.blockNumberEndInt >= blockInt)
            .ToListAsync()
            |> Async.AwaitTask

    let getTokenInfos (triggerResults: triggerResults seq) =
        async {
                 let pairAddresses = triggerResults 
                                        |> Seq.map (fun x -> x.pairAddress)
                 let! infos = ethDB.
                                tokenInfoEntities.
                                Where(fun x -> pairAddresses.Contains(x.AddressPair)).
                                ToListAsync()
                                |> Async.AwaitTask
                 
                 let r = triggerResults
                         |> Seq.map (fun x -> 
                             let info = infos
                                        |> Seq.tryFind (fun y -> y.AddressPair = x.pairAddress)

                             match info with
                             | Some i -> { x with 
                                                nameLong = i.NameLong
                                                nameShort = i.NameShort
                                                totalSupply = i.TotalSupply
                                                }
                             | None -> x
                         )

                 return r
               }
    let updateLatestTrigger (lastBlock: int) =
        let trigger = TriggerHistory()
        trigger.title <- scopedNames.trigger_5mins5percOfMK_Name
        trigger.blockNumberEndInt <- lastBlock
        ethDB.triggerHistoriesEntities.Add(trigger) |> ignore
        ethDB.SaveChangesAsync() |> Async.AwaitTask

    let mapToTriggerResult (swap: SwapsETH_Token) = 
        let ethInUsdSum = BigDecimal.Parse(swap.EthIn.ToZero()) * BigDecimal.Parse(string swap.priceETH_USD)  
        let ethOutUsdSum = BigDecimal.Parse(swap.EthOut.ToZero()) * BigDecimal.Parse(string swap.priceETH_USD)

        let res = 
                  { 
                            pairAddress = swap.pairAddress
                            priceDifference = BigDecimal.Parse("".ToZero()) 
                            volumeInUsd = ethInUsdSum
                            priceETH_USD = swap.priceETH_USD
                            ethInUsdSum = ethInUsdSum
                            ethOutUsdSum = ethOutUsdSum
                            nameLong = ""
                            nameShort = ""
                            totalSupply = ""
                            priceTokenInETH = swap.priceTokenInETH
                  }
        res
        
    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                let countIn5minPeriods = 12 * 24 * 1 // 1 day

                let! lastBlock = getLastProcessedBlock()
                let! latestTrigger = getLatestTrigger()

                if lastBlock - latestTrigger < chainSettingsOption.BlocksIn5Minutes
                then
                    return ()
                else
                    let! currentPeriod = getTxnsForPeriod( lastBlock)
                    let a = 1
                    return ()
                                              
                    do! currentPeriod 
                        |>   ( Seq.map mapToTriggerResult
                                >> getTokenInfos
                                >> Async.map (Seq.filter (fun x -> 
                                                            x.mkBigDec > BigDecimal.Parse ("1") && 
                                                            x.volumeInUsd > ( x.mkBigDec * BigDecimal.Parse ("0.05"))))
                                >> Async.map (Seq.map (fun x -> x))
                                >> Async.Bind scoped_telegram.sendMessages_trigger_5mins5percOfMK
                                )
                        |> Async.Ignore


                    do! updateLatestTrigger lastBlock 
                        |> Async.Ignore
            }