﻿module scoped_trigger_5mins

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.Options

open ChainSettingsOptionModule
open IScopedProcessingService
open Extensions
open scoped_telegram
open bl_others

open alchemy
open ethCommonDB
open ethCommonDB.models
open Nethereum.Util

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

    let avarage countIn5minPeriods (swaps: string * SwapsETH_Token seq) = 
        let (pairAddress, swap) = swaps

        let ethInSum = swap 
                      |> Seq.fold (fun acc x -> 
                                acc + BigDecimal.Parse(x.EthIn.ToZero()) * BigDecimal.Parse(string x.priceETH_USD) ) (BigDecimal.Parse("0"))
        let ethOutSum = swap 
                      |> Seq.fold (fun acc x -> 
                                acc + BigDecimal.Parse(x.EthOut.ToZero()) * BigDecimal.Parse(string x.priceETH_USD) ) (BigDecimal.Parse("0"))

        let res = { ethInUsdAverage = ethInSum / BigDecimal.Parse( countIn5minPeriods |> string)
                    ethInUsdSum = ethInSum  
                    ethOutUsdSum = ethOutSum
                    pairAddress = pairAddress 
                    priceETH_USD = (Seq.head swap).priceETH_USD
                    priceTokenInETH = (Seq.head swap).priceTokenInETH
                  }
        res


    let comparePrices (v: swapT seq * swapT seq)  =
        let prevNPeriod, currentPeriod = v
        let koef = BigDecimal.Parse("4")
        currentPeriod
        |> Seq.choose (fun currentPeriodElem ->
            match prevNPeriod 
                    |> Seq.tryFind (fun prevNPeriodElem -> prevNPeriodElem.pairAddress = currentPeriodElem.pairAddress) 
            with
            | Some prevNPeriodElem ->
                let priceDifference = currentPeriodElem.ethInUsdSum / prevNPeriodElem.ethInUsdAverage

                if currentPeriodElem.ethInUsdSum > currentPeriodElem.ethOutUsdSum && 
                   priceDifference > koef
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
        | [ true, currentPeriod; false, prevNPeriod ] -> prevNPeriod, currentPeriod
        | [ false, prevNPeriod; true, currentPeriod ] -> prevNPeriod, currentPeriod
        | _ -> [], []          

    let transformPeriod countIn5minPeriods : SwapsETH_Token seq -> swapT seq  =
        //Seq.filter (fun x -> not (x.EthIn = ""))
        Seq.groupBy (fun entity-> entity.pairAddress) 
        >> Seq.map (avarage countIn5minPeriods)
        >> Seq.filter (fun x -> x.ethInUsdAverage > 1) 
            
    let transformPeriods countIn5minPeriods (lst:(SwapsETH_Token seq * SwapsETH_Token seq) ) =
        let prevNPeriod, currentPeriod = lst
        let aa = currentPeriod |> Seq.distinctBy (fun x -> x.pairAddress) |> Seq.length
        (prevNPeriod |> transformPeriod countIn5minPeriods, currentPeriod |> transformPeriod countIn5minPeriods)
    
    let updateLatestTrigger (lastBlock: int) =
        let trigger = TriggerHistory()
        trigger.title <- scopedNames.trigger_5mins_Name
        trigger.blockNumberEndInt <- lastBlock
        ethDB.triggerHistoriesEntities.Add(trigger) |> ignore
        ethDB.SaveChangesAsync() |> Async.AwaitTask

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

    let trigger lastBlock countIn5minPeriods = 
                Async.map ( List.ofSeq
                            >> List.groupBy (fun (entity:SwapsETH_Token)-> entity.blockNumberEndInt = lastBlock)
                            >> splitList
                            >> (transformPeriods countIn5minPeriods)
                            >> comparePrices
                            )
                >> Async.Bind getTokenInfos
                >> Async.Bind scoped_telegram.sendMessages_trigger_5min

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) (value: int) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                let countIn5minPeriods = 12

                let! lastBlock = getLastProcessedBlock()
                let! latestTrigger = getLatestTrigger()

                if lastBlock - latestTrigger < chainSettingsOption.BlocksIn5Minutes
                then
                    logger.LogWarning($"scoped_trigger_5mins - Number of blocks in DB {lastBlock - latestTrigger} is less than desired number of blocks {chainSettingsOption.BlocksIn5Minutes}")
                    return ()

                else
                    let periods = getTxnsForPeriod( lastBlock - chainSettingsOption.BlocksIn5Minutes * countIn5minPeriods)

                    do! trigger lastBlock countIn5minPeriods periods
                        |> Async.Ignore

                    do! updateLatestTrigger lastBlock 
                        |> Async.Ignore
            }