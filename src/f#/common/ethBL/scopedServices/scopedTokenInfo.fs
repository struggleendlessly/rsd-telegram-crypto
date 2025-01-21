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
open ethCommonDB.models
open System.Numerics
open responseEthCall
open System.Globalization

type tokenInfoTemp = 
    {   AddressToken: string
        TotalSupply: uint64}

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
                        |> Async.map (Array.choose id)
                        //|> Async.map (Array.filter (fun x ->not (Array.contains x.AddressToken0 chainSettingsOption.ExcludedAddresses ) ||
                        //                                    not (Array.contains x.AddressToken1 chainSettingsOption.ExcludedAddresses )))
      
            return t1
        }

    let getTokensWithoutTotalSupply () =
        let noBlock = TokenInfo.Default()
        let getAddrToken (x: TokenInfo) = x.AddressToken

        let r = 
            ethDB.tokenInfoEntities
                .Where(fun x -> x.TotalSupply = 0UL)
                .Take(100)
                .ToListAsync()
                |> Async.AwaitTask
                |> Async.map (Option.ofObj   
                                  >> Option.defaultValue noBlock
                                  >> Seq.distinctBy (fun elem -> elem.AddressToken)
                                  >> Seq.toArray
                                )
        r
    let convertToTokenSupply (ti: TokenInfo []) (ts: responseEthCall) = 

        let aa = ts.result
        match ts.error with
        | None -> let num = BigInteger.Parse(ts.result.Substring(2), NumberStyles.AllowHexSpecifier)
                  let token = ti |> Array.find (fun x -> x.AddressToken = ts.id)  
                  let ulong1 = num / BigInteger.Pow(10I, token.Decimals) |> uint64 // to small !!!!!!!!!!!
                  let res = { AddressToken= ts.id
                              TotalSupply= ulong1}
                  Some (res)
        | Some e -> None

    let mapToTokenInfo  (ti:TokenInfo seq) (ts: tokenInfoTemp) = 
                    ti
                    |> Seq.filter (fun x -> x.AddressToken = ts.AddressToken)
                    |> Seq.iter (fun x -> x.TotalSupply <- ts.TotalSupply)
        

    let saveTotalSupply (a: tokenInfoTemp []) = 
        async{
                let addrSet = a 
                               |> Array.map (fun x -> x.AddressToken) 
                               |> Set.ofArray

                let enteties = ethDB
                                    .tokenInfoEntities
                                    .Where(fun x -> addrSet.Contains( x.AddressToken))
                                    .ToListAsync()
                                    |> Async.AwaitTask
                                    |> Async.RunSynchronously
                a 
                |> Array.iter (fun x -> mapToTokenInfo enteties x)

                ethDB.tokenInfoEntities.UpdateRange(enteties)
                ethDB.SaveChangesAsync() 
                            |> Async.AwaitTask
                            |> Async.RunSynchronously
                            |> ignore
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

                let! d = getTokensWithoutTotalSupply ()

                let aa = d |> Array.map (fun x -> x.AddressToken)
                           |> alchemy.getTotalSupply
                           |> Async.map( Array.collect id )
                           |> Async.map (Array.map (convertToTokenSupply d))
                           |> Async.map (Array.choose id)
                           |> Async.Bind saveTotalSupply
                           |> Async.RunSynchronously


                return ()
            }