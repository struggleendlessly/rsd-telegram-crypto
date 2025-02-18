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
open responseTokenMetadata

type tokenInfoTemp = 
    {   AddressToken: string
        TotalSupply: BigInteger
        NameShort: string
        NameLong: string
    }

type scopedTokenInfo(
        logger: ILogger<scopedTokenInfo>,
        alchemy: alchemyEVM,
        chainSettingsOption:  IOptions<ChainSettingsOption>,
        ethDB: IEthDB) =

    let chainSettingsOption = chainSettingsOption.Value;

    let getTokens0and1 (addressPair: string seq) =
        ethDB.tokenInfoEntities
            .Where(fun x -> addressPair.Contains(x.AddressPair))
            .ToListAsync()
            |> Async.AwaitTask

    let addToken0and1 (addressPair: string seq) =
        async {
            let! withoutT0T1inDB = 
                        addressPair
                        |> getTokens0and1
                        |> Async.map (fun x -> x |> Seq.map (fun x -> x.AddressPair))
                        |> Async.map (fun address -> Seq.except address addressPair)

            let! t0 = alchemy.getEthCall_token0 withoutT0T1inDB
                        |> Async.map (Seq.collect id)
                        |> Async.map (Seq.map mapResponseEthCall.mapToken0)

            let! t1 = alchemy.getEthCall_token1 withoutT0T1inDB
                        |> Async.map (Seq.collect id)
                        |> Async.map (Seq.map (mapResponseEthCall.mapToken1 t0))
                        |> Async.map (Seq.map (mapResponseEthCall.mapToken01toAddress chainSettingsOption.ExcludedAddresses))
                        |> Async.map (Seq.choose id)
                        //|> Async.map (Array.filter (fun x ->not (Array.contains x.AddressToken0 chainSettingsOption.ExcludedAddresses ) ||
                        //                                    not (Array.contains x.AddressToken1 chainSettingsOption.ExcludedAddresses )))
      
            return t1
        }

    let getTokensWithoutTotalSupply () =
        let noBlock = TokenInfo.Default()
        let getAddrToken (x: TokenInfo) = x.AddressToken

        let r = 
            ethDB.tokenInfoEntities
                .Where(fun x -> x.TotalSupply = "")
                .Take(500)
                .ToListAsync()
                |> Async.AwaitTask
                |> Async.map (Option.ofObj   
                                  >> Option.defaultValue noBlock
                                  >> Seq.distinctBy (fun elem -> elem.AddressToken)
                                  >> Seq.toArray
                                )
        r

    let getTokensWithoutNames () =
        let noBlock = TokenInfo.Default()
        let getAddrToken (x: TokenInfo) = x.AddressToken

        let r = 
            ethDB.tokenInfoEntities
                .Where(fun x -> x.NameLong = "")
                .Take(500)
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
        | None -> if ts.result.CompareCI("0x") 
                  then
                      None
                  else
                      let num = BigInteger.Parse(ts.result.Substring(2), NumberStyles.AllowHexSpecifier)
                      let token = ti |> Array.find (fun x -> x.AddressToken = ts.id)  
                      let ulong1 = num / BigInteger.Pow(10I, token.Decimals) 
                      let res = { AddressToken= ts.id
                                  TotalSupply= ulong1
                                  NameLong= ""
                                  NameShort= ""
                                 }
                      Some (res)
        | Some e -> None

    let convertToTokenNames (ti: TokenInfo []) (ts: responseTokenMetadata) = 

        let aa = ts.result
        match ts.error with
        | None -> 
                  let res = { AddressToken= ts.id
                              TotalSupply= BigInteger.Zero
                              NameLong= ts.result.name
                              NameShort= ts.result.symbol
                             }
                  Some (res)
        | Some e -> None

    let mapToTokenInfoSupply  (ti:TokenInfo seq) (ts: tokenInfoTemp) = 
                    ti
                    |> Seq.filter (fun x -> x.AddressToken = ts.AddressToken)
                    |> Seq.iter (fun x -> x.TotalSupply <- ts.TotalSupply.ToString())
        
    let mapToTokenInfoNames  (ti:TokenInfo seq) (ts: tokenInfoTemp) = 
            ti
            |> Seq.filter (fun x -> x.AddressToken = ts.AddressToken)
            |> Seq.iter (fun x -> x.NameShort <- ts.NameShort
                                  x.NameLong <- ts.NameLong)     

    let saveTokenNames (a: tokenInfoTemp seq) = 
        async{
                let addrSet = a 
                               |> Seq.map (fun x -> x.AddressToken) 

                let! enteties = ethDB
                                    .tokenInfoEntities
                                    .Where(fun x -> addrSet.Contains( x.AddressToken))
                                    .ToListAsync()
                                    |> Async.AwaitTask
                a 
                |> Seq.iter (fun x -> mapToTokenInfoNames enteties x)

                ethDB.tokenInfoEntities.UpdateRange(enteties)
                do! ethDB.SaveChangesAsync() 
                            |> Async.AwaitTask
                            |> Async.Ignore

        }

    let saveTotalSupply (a: tokenInfoTemp seq) = 
        async{
                let addrSet = a 
                               |> Seq.map (fun x -> x.AddressToken)

                let! enteties = ethDB
                                    .tokenInfoEntities
                                    .Where(fun x -> addrSet.Contains( x.AddressToken))
                                    .ToListAsync()
                                    |> Async.AwaitTask
                a 
                |> Seq.iter (fun x -> mapToTokenInfoSupply enteties x)

                ethDB.tokenInfoEntities.UpdateRange(enteties)
                do! ethDB.SaveChangesAsync() 
                            |> Async.AwaitTask
                            |> Async.Ignore
        }

    let addTokenNames d = 
            Seq.map (fun (x:TokenInfo) -> x.AddressToken)
            >> alchemy.getTokenNames
            >> Async.map( Seq.collect id 
                          >> Seq.map (convertToTokenNames d)
                          >> Seq.choose id)
            >> Async.Bind saveTokenNames

    let addTotalSupply d = 
            Seq.map (fun (x:TokenInfo) -> x.AddressToken)
            >> alchemy.getTotalSupply
            >> Async.map( Seq.collect id 
                          >> Seq.map (convertToTokenSupply d)
                          >> Seq.choose id)
            >> Async.Bind saveTotalSupply
                
    member this.getToken0and1(addressPair: string seq) =
        async {
            let! newT0T1 =  addToken0and1 addressPair 
            ethDB.tokenInfoEntities.AddRangeAsync newT0T1 
                    |> Async.AwaitTask 
                    |> ignore 

            let! _ = ethDB.SaveChangesAsync() 
                    |> Async.AwaitTask 

            return! getTokens0and1 addressPair
        }

    member this.getDecimals (addressPair: string seq): Async<Map<string, int>> =
        async {
            let! enteties = ethDB.tokenInfoEntities
                                .Where(fun x ->  x.Decimals = 0 && not (x.AddressToken = ""))
                                .ToListAsync()
                                |> Async.AwaitTask
                            //|> Async.RunSynchronously
            let! decimals =    
                             enteties
                                |> Seq.map (fun x -> x.AddressToken)
                                |> alchemy.getEthCall_decimals 
                                //|> Async.RunSynchronously
            let a = decimals
                    |> Seq.collect id
                    |> Seq.map (mapResponseEthCall.mapDecimals enteties)

            do! ethDB.SaveChangesAsync() 
                |> Async.AwaitTask
                |> Async.Ignore

            let! r = ethDB.tokenInfoEntities
                        .Where(fun x -> addressPair.Contains(x.AddressPair) && x.Decimals > 0)
                        .ToListAsync()
                        |> Async.AwaitTask
                        |> Async.map (fun x -> x |> Seq.map (fun x -> (x.AddressPair, x.Decimals ))|> Map.ofSeq)
            return r
        }

    interface IScopedProcessingService with
        member _.DoWorkAsync(ct: CancellationToken) (value: int) =
            task {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now)

                let! ts = getTokensWithoutTotalSupply ()
                do! ts |> addTotalSupply ts

                let! names = getTokensWithoutNames ()
                do! names |> addTokenNames names 
            }