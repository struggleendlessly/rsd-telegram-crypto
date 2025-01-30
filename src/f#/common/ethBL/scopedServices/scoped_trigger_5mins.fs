module scoped_trigger_5mins

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

type scoped_trigger_5mins(
        logger: ILogger<scoped_trigger_5mins>,
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
            .Where(fun x -> x.title = scopedNames.trigger_5mins_Name)
            .OrderByDescending(fun x -> x.blockNumberEndInt)
            .Take(1)
            .FirstOrDefaultAsync()
            |> Async.AwaitTask
            |> Async.map (Option.ofObj 
                >> Option.defaultValue noBlock
                >> getNumberInt)

    let getTxnsForPeriod (firstBlockInPeriod) =
        ethDB.swapsETH_TokenEntities
            .Where(fun x -> x.blockNumberEndInt >= firstBlockInPeriod)
            .ToListAsync()
            |> Async.AwaitTask

    let avarage (swaps: string * SwapsETH_Token []) = 
        let (pairAddress, swap) = swaps

        let ethInSum = swap 
                      |> Array.fold (fun acc x -> 
                                acc + BigDecimal.Parse(x.EthIn.ToZero()) * BigDecimal.Parse(string x.priceETH_USD) ) (BigDecimal.Parse("0"))
        let ethOutSum = swap 
                      |> Array.fold (fun acc x -> 
                                acc + BigDecimal.Parse(x.EthOut.ToZero()) * BigDecimal.Parse(string x.priceETH_USD) ) (BigDecimal.Parse("0"))

        let res = { ethInUsdAverage = ethInSum / BigDecimal.Parse(string swap.Length)
                    ethInUsdSum = ethInSum  
                    ethOutUsdSum = ethOutSum
                    pairAddress = pairAddress 
                    priceETH_USD = (Array.head swap).priceETH_USD
                    priceTokenInETH = (Array.head swap).priceTokenInETH
                  }
        res


    let comparePrices (v: swapT[] * swapT[])  =
        let prevNPeriod, currentPeriod = v

        currentPeriod
        |> Array.choose (fun currentPeriodElem ->
            match prevNPeriod 
                    |> Array.tryFind (fun prevNPeriodElem -> prevNPeriodElem.pairAddress = currentPeriodElem.pairAddress) 
            with
            | Some firstInPeriodElem ->
                let priceDifference = currentPeriodElem.ethInUsdAverage / firstInPeriodElem.ethInUsdAverage
                if currentPeriodElem.ethInUsdSum > currentPeriodElem.ethOutUsdSum && priceDifference > 4.0
                then
                    Some { 
                            pairAddress = currentPeriodElem.pairAddress
                            priceDifference = priceDifference 
                            volumeInUsd = currentPeriodElem.ethInUsdAverage
                            priceETH_USD = currentPeriodElem.priceETH_USD
                            ethInUsdSum = currentPeriodElem.ethInUsdSum
                            ethOutUsdSum = currentPeriodElem.ethOutUsdSum
                            nameLong = ""
                            nameShort = ""
                            totalSupply = ""
                            priceTokenInETH = currentPeriodElem.priceTokenInETH
                         }
                else
                    None
            | None -> None
        )

    let splitList grouped = 
        match grouped with
        | [| true, lastInPeriod; false, firstInPeriod |] -> firstInPeriod, lastInPeriod
        | [| false, firstInPeriod; true, lastInPeriod |] -> firstInPeriod, lastInPeriod
        | _ -> [||], [||]

    let transformPeriod  : SwapsETH_Token [] -> swapT []  =
        Array.filter (fun x -> not (x.EthIn = ""))
        >> Array.groupBy (fun entity-> entity.pairAddress) 
        >> Array.map avarage
        >> Array.filter (fun x -> x.ethInUsdAverage > 0) 
            
    let transformPeriods (lst:(SwapsETH_Token [] * SwapsETH_Token []) ) =
        let firstInPeriod, lastInPeriod = lst
        (firstInPeriod |> transformPeriod, lastInPeriod |> transformPeriod)
    
    let updateLatestTrigger (lastBlock: int) =
        let trigger = TriggerHistory()
        trigger.title <- scopedNames.trigger_5mins_Name
        trigger.blockNumberEndInt <- lastBlock
        ethDB.triggerHistoriesEntities.Add(trigger) |> ignore
        ethDB.SaveChangesAsync() |> Async.AwaitTask

    let getTokenInfos (triggerResults: triggerResults []) =
        async {
                 let pairAddresses = triggerResults 
                                        |> Array.map (fun x -> x.pairAddress)
                 let! infos = ethDB.
                                tokenInfoEntities.
                                Where(fun x -> pairAddresses.Contains(x.AddressPair)).
                                ToListAsync()
                                |> Async.AwaitTask
                 
                 let r = triggerResults
                         |> Array.map (fun x -> 
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

    let trigger lastBlock = 
                Async.map ( List.ofSeq
                            >> List.toArray 
                            >> Array.groupBy (fun (entity:SwapsETH_Token)-> entity.blockNumberEndInt = lastBlock)
                            >> splitList
                            >> transformPeriods
                            >> comparePrices
                            )
                >> Async.Bind getTokenInfos
                >> Async.Bind scoped_telegram.sendMessages_trigger_5min

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                let countInPeriods = 12

                let! lastBlock = getLastProcessedBlock()
                let! latestTrigger = getLatestTrigger()

                if lastBlock - latestTrigger < chainSettingsOption.BlocksIn5Minutes
                then
                    return ()
                else
                    let periods = getTxnsForPeriod( lastBlock - chainSettingsOption.BlocksIn5Minutes * countInPeriods)

                    do! trigger lastBlock periods
                        |> Async.Ignore

                    do! updateLatestTrigger lastBlock 
                        |> Async.Ignore
            }