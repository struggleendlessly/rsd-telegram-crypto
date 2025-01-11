module scoped_trigger_5mins

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open ChainSettingsOptionModule
open IScopedProcessingService
open Extensions
open responseSwap

open alchemy
open Microsoft.Extensions.Options
open ethCommonDB
open ethCommonDB.models
open Nethereum.Util

type swapT =
    {
        ethInUsd: BigDecimal
        pairAddress: string
    }
    
let empty_swapT =
    {
        ethInUsd = BigDecimal.Parse("0")
        pairAddress = ""
    }
type Comparison = { 
    pairAddress: string
    priceDifference: BigDecimal 
    volumeInUsd: BigDecimal
    }

type scoped_trigger_5mins(
        logger: ILogger<scoped_trigger_5mins>,
        alchemy: alchemyEVM,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
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


    let comparePrices (firstArray: swapT[]) (secondArray: swapT[]) =
        firstArray
        |> Array.choose (fun firstElem ->
            match secondArray 
                    |> Array.tryFind (fun secondElem -> secondElem.pairAddress = firstElem.pairAddress) 
            with
            | Some secondElem ->
                let priceDifference = firstElem.ethInUsd / secondElem.ethInUsd
                Some { 
                    pairAddress = firstElem.pairAddress
                    priceDifference = priceDifference 
                    volumeInUsd = firstElem.ethInUsd
                    }
            | None -> None
        )

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                let countInPeriods = 5

                let! a = getLastProcessedBlock()
                    
                let! e = getTxnsForPeriod( a - chainSettingsOption.BlocksIn5Minutes * countInPeriods)
                let ae = e |> List.ofSeq |> List.toArray
                let grouped = ae |> Array.groupBy (fun entity-> entity.blockNumberEndInt = a) 
                
                let firstInPeriod, lastInPeriod =
                    match grouped with
                    | [| true, firstGroup; false, secondGroup |] -> secondGroup, firstGroup
                    | [| false, secondGroup; true, firstGroup |] -> secondGroup, firstGroup
                    | _ -> [||], [||]

                let averageF = firstInPeriod 
                                |> Array.filter (fun x -> not (x.EthIn = ""))
                                |> Array.groupBy (fun entity-> entity.pairAddress) 
                                |> Array.map avarage
                                |> Array.filter (fun x -> x.ethInUsd > 0)

                let averageL = lastInPeriod 
                                |> Array.filter (fun x -> not (x.EthIn = ""))
                                |> Array.groupBy (fun entity-> entity.pairAddress) 
                                |> Array.map avarage

                let comparisons = comparePrices averageL averageF

                return ()
            }