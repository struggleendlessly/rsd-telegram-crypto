module scopedTokenInfo

open System
open System.Threading
open System.Linq

open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open IScopedProcessingService
open Extensions
open responseSwap

open alchemy
open dbMigration
open dbMigration.models

type scopedTokenInfo(
        logger: ILogger<scopedTokenInfo>,
        alchemy: alchemy,
        ethDB: ethDB)
         as this =

    //member this.getTokenAddress (param: (string * string) array) =
    //    let tupleMap = Map.ofArray param
    //    let addressPair = param |> Array.map (fun (pairAddr, hash) -> pairAddr)
    //    let diff = ethDB.EthTokenInfoEntities
    //                .Where(fun x -> addressPair.Contains(x.AddressPair))
    //                .ToListAsync()
    //                |> Async.AwaitTask
    //                |> Async.map (fun x -> x |> Seq.map (fun x -> x.AddressPair))
    //                |> Async.map (fun address -> Seq.except address addressPair)
    //                |> Async.Bind (fun diff -> 
    //                                    let filteredParam = param |> Array.filter (fun (pairAddr, hash) -> not (diff |> Seq.contains pairAddr))
    //                                    alchemy.eth_getTransactionReceipt filteredParam
    //                               )
    //                |> Async.RunSynchronously

    //    let a = diff
    //            |> Array.collect id
    //            |> Array.map (fun x -> 
    //                                Array.take 3 x.result.logs 
    //                                |> Array.filter (fun v ->    
    //                                                        not (String.Equals(v.address, x.id, StringComparison.InvariantCultureIgnoreCase)) && 
    //                                                        not (Array.contains v.address ethExcludeTokens.tokensExclude )
    //                                                   )    
    //                                 |> Array.map (fun c -> c.address)
    //                                 |> Array.distinct
    //                            )
    //    a

    member this.getToken0and1(addressPair: string[]) =
         let diff = ethDB.EthTokenInfoEntities
                        .Where(fun x -> addressPair.Contains(x.AddressPair))
                        .ToListAsync()
                        |> Async.AwaitTask
                        |> Async.map (fun x -> x |> Seq.map (fun x -> x.AddressPair))
                        |> Async.map (fun address -> Seq.except address addressPair)
                        |> Async.RunSynchronously

         let par = diff |> Seq.toArray
         let t0 = alchemy.getEthCall_token0 par 
                    |> Async.RunSynchronously
                    |> Array.collect id
                    |> Array.map mapResponseEthCall.mapToken0

         let t1 = alchemy.getEthCall_token1 par  
                    |> Async.RunSynchronously
                    |> Array.collect id
                    |> Array.map (mapResponseEthCall.mapToken1 t0)
                    |> Array.map mapResponseEthCall.mapToken01toAddress
         let elementsToExcluude = 
                    t1
                    |> Array.filter (fun x ->not (Array.contains x.AddressToken0 ethExcludeTokens.tokensExclude ) ||
                                             not (Array.contains x.AddressToken1 ethExcludeTokens.tokensExclude ))
         let filteredArray1 = t1 |> Array.filter (fun x -> not (Array.contains x elementsToExcluude))
         let a = elementsToExcluude |> ethDB.EthTokenInfoEntities.AddRangeAsync                  
         
         ethDB.SaveChangesAsync() 
            |> Async.AwaitTask
            |> Async.RunSynchronously
            |> ignore

         let tokens0and1 = ethDB.EthTokenInfoEntities
                            .Where(fun x -> addressPair.Contains(x.AddressPair))
                            .ToListAsync()
                             |> Async.AwaitTask
                             |> Async.RunSynchronously
         tokens0and1

    member this.getDecimals (addressPair: string[]) =
        let enteties = ethDB.EthTokenInfoEntities
                        .Where(fun x ->  x.Decimals = 0 && not (x.AddressToken = ""))
                        .ToListAsync()
                        |> Async.AwaitTask
                        |> Async.RunSynchronously
        let decimals =    
                         enteties
                            |> Seq.map (fun x -> x.AddressToken)
                            |> Seq.toArray
                            |> alchemy.getEthCall_decimals 
                            |> Async.RunSynchronously
        let a = decimals
                |> Array.collect id
                |> Array.map (mapResponseEthCall.mapDecimals enteties)

        ethDB.SaveChangesAsync() 
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore

        ethDB.EthTokenInfoEntities
                    .Where(fun x -> addressPair.Contains(x.AddressPair))
                    .ToListAsync()
                    |> Async.AwaitTask
                    |> Async.map (fun x -> x |> Seq.map (fun x -> (x.AddressPair, x.Decimals ))|> Map.ofSeq)
        
    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                    

                return ()
            }