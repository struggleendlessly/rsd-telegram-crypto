module scoped_swapsTokensToNhours

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
open System.Numerics
open responseEthCall
open System.Globalization
open responseTokenMetadata
open bl_createSeq
open Extensions
open Nethereum.Util

type SwapsETH_TokenGroup = 
    {
        startBlock: int
        step: int
        data: SwapsETH_Token seq
    }

type scoped_swapsTokens5minTo30mins(
        logger: ILogger<scoped_swapsTokens5minTo30mins>,
        alchemy: alchemyEVM,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;

    let getLastEthBlock_30mins () =

        let noBlock = SwapsETH_Token_30mins.Default(int chainSettingsOption.DefaultBlockNumber)
        let getNumberInt (x: SwapsETH_Token_30mins) = x.blockNumberEndInt

        ethDB.swapsETH_Token_30MinsEntities
             .OrderByDescending(fun x -> x.blockNumberEndInt)
             .FirstOrDefaultAsync()
             |> Async.AwaitTask
             |> Async.map (Option.ofObj 
                                >> Option.defaultValue noBlock
                                >> getNumberInt)

    let getLastEthBlock_5mins () =
        let noBlock = SwapsETH_Token.Default(int chainSettingsOption.DefaultBlockNumber)
        let getNumberInt (x: SwapsETH_Token) = x.blockNumberEndInt

        ethDB.swapsETH_TokenEntities
                .OrderByDescending(fun x -> x.blockNumberEndInt)
                .FirstOrDefaultAsync()
                |> Async.AwaitTask
                |> Async.map (Option.ofObj 
                                >> Option.defaultValue noBlock
                                >> getNumberInt)

    let getDataFromDB (step: int) (start: int) =
        async{
            let! t  = ethDB.swapsETH_TokenEntities
                                    .OrderByDescending(fun x -> x.blockNumberEndInt)
                                    .Where(fun x -> x.blockNumberEndInt >= start && x.blockNumberEndInt < (start + step))
                                    .ToListAsync()
                                    |> Async.AwaitTask
            let res = 
                    {
                         startBlock = start
                         step = step
                         data = t :> seq<_>
                    }

            return res
        }
    //let getDataFromDB (start: int) (end1: int) =
    //    async{
    //        let! res  = ethDB.swapsETH_TokenEntities
    //                                .OrderByDescending(fun x -> x.blockNumberEndInt)
    //                                .Where(fun x -> x.blockNumberEndInt >= start && x.blockNumberEndInt < end1)
    //                                .ToListAsync()
    //                                |> Async.AwaitTask
    //        return res
    //    }  
    let processToken (a:SwapsETH_TokenGroup) = 
        let minPriceUsd = a.data 
                            |> Seq.minBy (fun x -> (x.priceETH_USD |> decimal) * x.priceTokenInETH) 
                            |> (fun x -> (x.priceETH_USD |> decimal) * x.priceTokenInETH)
        let maxPriceUsd = a.data 
                            |> Seq.maxBy (fun x -> (x.priceETH_USD |> decimal) * x.priceTokenInETH) 
                            |> (fun x -> (x.priceETH_USD |> decimal) * x.priceTokenInETH)
        
        let res = new SwapsETH_Token_30mins()
        res.blockNumberStartInt <- a.startBlock
        res.blockNumberEndInt <- a.startBlock + a.step - 1
        res.priceTokenInUSD_min <- Decimal.Round(minPriceUsd, 15, MidpointRounding.AwayFromZero)
        res.priceTokenInUSD_max <- Decimal.Round(maxPriceUsd, 15, MidpointRounding.AwayFromZero)
        res.priceTokenInUSD_avr <- Decimal.Round((minPriceUsd + maxPriceUsd) / 2.0M, 15, MidpointRounding.AwayFromZero)

        res.pairAddress <- a.data |> Seq.head |> (fun x -> x.pairAddress) 

        let ethInSum = a.data |> Seq.fold (fun  acc (x:SwapsETH_Token) ->  BigDecimal.Parse (x.EthIn.ToZero()) + acc) (BigDecimal.Parse "0")
        let ethOutSum = a.data |> Seq.fold (fun  acc (x:SwapsETH_Token) ->  BigDecimal.Parse (x.EthOut.ToZero()) + acc) (BigDecimal.Parse "0")
        let tokenInSum = a.data |> Seq.fold (fun  acc (x:SwapsETH_Token) ->  BigDecimal.Parse (x.TokenIn.ToZero()) + acc) (BigDecimal.Parse "0")
        let tokenOutSum = a.data |> Seq.fold (fun  acc (x:SwapsETH_Token) ->  BigDecimal.Parse (x.TokenOut.ToZero()) + acc) (BigDecimal.Parse "0")

        res.EthIn <- ethInSum.ToString15()
        res.EthOut <- ethOutSum.ToString15()
        res.TokenIn <- tokenInSum.ToString15()
        res.TokenOut <- tokenOutSum.ToString15()

        if ethInSum > 0 && tokenOutSum > 0 then
            res.isBuyToken <- true

        if ethOutSum > 0 && tokenInSum > 0 then
            res.isBuyEth <- true

        res

    let processRange (a: SwapsETH_TokenGroup) = 
        let e =
            a.data
            |> Seq.groupBy (fun x -> x.pairAddress)
            |> Seq.map (fun (key, data) -> processToken { a with data = data })
            |> Seq.filter (fun (x:SwapsETH_Token_30mins)-> x.priceTokenInUSD_avr < 100.0M)
        e

    let saveToDB items =             
        async {
                do! ethDB.swapsETH_Token_30MinsEntities.AddRangeAsync(items) |> Async.AwaitTask
                let! result = ethDB.SaveChangesAsync() |> Async.AwaitTask
                return result // Return the result of SaveChangesAsync()
        }

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) (value: int) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                //let! a30 = getLastEthBlock_30mins()
                //let! a5 = getLastEthBlock_5mins()
                let blocksIn30min = chainSettingsOption.BlocksIn5Minutes * 6
                let! b = getSeqToProcess (blocksIn30min * 10) blocksIn30min getLastEthBlock_30mins getLastEthBlock_5mins
                let! e = b 
                        |> Seq.map ( 
                                    getDataFromDB blocksIn30min
                                    >> Async.map processRange 
                                    >> Async.Bind (Seq.toArray >> saveToDB) 
                                    ) 

                        |> Async.Sequential

                           //|> Async.map (Seq.map (getDataFromDB blocksIn30min))
                           //|> Async.Parallel
                // get last 30 mins (6 5mins)
                // calculate the sum for swaps
                // calculate min/max/avg token price
                // save to DB
                return e
            }