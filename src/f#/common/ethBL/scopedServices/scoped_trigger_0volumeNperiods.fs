module scoped_trigger_0volumeNperiods

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

type scoped_trigger_0volumeNperiods(
        logger: ILogger<scoped_trigger_0volumeNperiods>,
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
            .Where(fun x -> x.title = scopedNames.trigger_0volumeNperiods_Name)
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

    let getTxnsForPrevNPeriods (blockIntStart) blockIntEnd =
        ethDB.swapsETH_TokenEntities
            .Where(fun x -> x.blockNumberEndInt >= blockIntStart && x.blockNumberEndInt < blockIntEnd)
            .Select(fun x -> x.pairAddress)
            .Distinct()
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
        trigger.title <- scopedNames.trigger_0volumeNperiods_Name
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
                let countIn5minPeriods = 12 * 12// * 0,5 // 1 day

                let! lastBlock = getLastProcessedBlock()
                let! latestTrigger = getLatestTrigger()

                if lastBlock - latestTrigger < chainSettingsOption.BlocksIn5Minutes
                then
                    return ()
                else
                    let! currentPeriod = getTxnsForPeriod( lastBlock)
                    let! prevNPeriods = getTxnsForPrevNPeriods (lastBlock - chainSettingsOption.BlocksIn5Minutes * countIn5minPeriods) lastBlock

                    let difference1 = currentPeriod 
                                        |>   (Seq.map (fun x -> x.pairAddress) 
                                              >> Seq.except prevNPeriods       )
                                              
                    let! difference = currentPeriod 
                                        |>   (Seq.map (fun x -> x.pairAddress) 
                                              >> Seq.except prevNPeriods
                                              >> Seq.map (fun x -> Seq.find (fun (q:SwapsETH_Token)-> q.pairAddress = x ) currentPeriod )
                                              >> Seq.map mapToTriggerResult
                                              >> getTokenInfos
                                              >> Async.Bind scoped_telegram.sendMessages_trigger_0volumeNperiods
                                              )

                    //let tokenInfos = getTokenInfo difference

                    do! updateLatestTrigger lastBlock 
                        |> Async.Ignore
            }