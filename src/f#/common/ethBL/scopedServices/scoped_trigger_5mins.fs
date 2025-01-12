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

    let getTxnsForPeriod (firstBlockInPeriod) =
        ethDB.swapsETH_TokenEntities
            .Where(fun x -> x.blockNumberEndInt >= firstBlockInPeriod)
            .ToListAsync()
            |> Async.AwaitTask

    let avarage (swaps: string * SwapsETH_Token []) = 
        let (pairAddress, swap) = swaps

        let a = swap 
                |> Array.fold (fun acc x -> 
                                acc + BigDecimal.Parse(x.EthIn) * BigDecimal.Parse(string x.priceETH_USD) ) (BigDecimal.Parse("0"))

        let res = { ethInUsd = a / BigDecimal.Parse(string swap.Length)
                    pairAddress = pairAddress }
        res


    let comparePrices (v: swapT[] * swapT[])  =
        let firstInPeriod, lastInPeriod = v

        lastInPeriod
        |> Array.choose (fun lastInPeriodElem ->
            match firstInPeriod 
                    |> Array.tryFind (fun firstInPeriodElem -> firstInPeriodElem.pairAddress = lastInPeriodElem.pairAddress) 
            with
            | Some secondElem ->
                let priceDifference = lastInPeriodElem.ethInUsd / secondElem.ethInUsd
                if priceDifference > 4.0
                then
                    Some { 
                            pairAddress = lastInPeriodElem.pairAddress
                            priceDifference = priceDifference 
                            volumeInUsd = lastInPeriodElem.ethInUsd
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
        >> Array.filter (fun x -> x.ethInUsd > 0) 
            
    let transformPeriods (lst:(SwapsETH_Token [] * SwapsETH_Token []) ) =
        let firstInPeriod, lastInPeriod = lst
        (firstInPeriod |> transformPeriod, lastInPeriod |> transformPeriod)

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                let countInPeriods = 5

                let! lastBlock = getLastProcessedBlock()
                    
                let grouped = getTxnsForPeriod( lastBlock - chainSettingsOption.BlocksIn5Minutes * countInPeriods)
                            |> Async.map ( List.ofSeq
                                           >> List.toArray 
                                           >> Array.groupBy (fun entity-> entity.blockNumberEndInt = lastBlock)
                                           >> splitList
                                           >> transformPeriods
                                           >> comparePrices)
                             |> Async.Bind scoped_telegram.sendMessages
                            
                             //|> Async.StartAsTask
                             |> Async.RunSynchronously

                return ()
            }