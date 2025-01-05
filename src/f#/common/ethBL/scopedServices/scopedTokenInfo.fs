module scopedTokenInfo

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


type scopedTokenInfo(
        logger: ILogger<scopedTokenInfo>,
        alchemy: alchemyEVM,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;

    let getTokens0and1 (addressPair: string[]) =
        ethDB.tokenInfoEntities
            .Where(fun x -> addressPair.Contains(x.AddressPair))
            .ToListAsync()
            |> Async.AwaitTask

    let addToken0and1 (addressPair: string[]) =
        async {
            let! withoutT0T1inDB = 
                        addressPair
                        |> getTokens0and1
                        |> Async.map (fun x -> x |> Seq.map (fun x -> x.AddressPair))
                        |> Async.map (fun address -> Seq.except address addressPair)
                        |> Async.map Seq.toArray

            let! t0 = alchemy.getEthCall_token0 withoutT0T1inDB
                        |> Async.map (Array.collect id)
                        |> Async.map (Array.map mapResponseEthCall.mapToken0)

            let! t1 = alchemy.getEthCall_token1 withoutT0T1inDB
                        |> Async.map (Array.collect id)
                        |> Async.map (Array.map (mapResponseEthCall.mapToken1 t0))
                        |> Async.map (Array.map (mapResponseEthCall.mapToken01toAddress chainSettingsOption.ExcludedAddresses))
                        |> Async.map (Array.filter (fun x ->not (Array.contains x.AddressToken0 chainSettingsOption.ExcludedAddresses ) ||
                                                            not (Array.contains x.AddressToken1 chainSettingsOption.ExcludedAddresses )))
      
            return t1
        }

    member this.getToken0and1(addressPair: string[]) =
        async {
            let! newT0T1 =  addToken0and1 addressPair 
            ethDB.tokenInfoEntities.AddRangeAsync newT0T1 
                    |> Async.AwaitTask 
                    |> ignore 

            let! _ = ethDB.SaveChangesAsync() 
                    |> Async.AwaitTask 

            return! getTokens0and1 addressPair
        }

    member this.getDecimals (addressPair: string[]) =
        let enteties = ethDB.tokenInfoEntities
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

        ethDB.tokenInfoEntities
                    .Where(fun x -> addressPair.Contains(x.AddressPair) && x.Decimals > 0)
                    .ToListAsync()
                    |> Async.AwaitTask
                    |> Async.map (fun x -> x |> Seq.map (fun x -> (x.AddressPair, x.Decimals ))|> Map.ofSeq)
        
    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)
                    

                return ()
            }