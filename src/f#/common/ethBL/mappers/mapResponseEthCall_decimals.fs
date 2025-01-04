module mapResponseEthCall

open System
open System.Linq

open responseEthCall
open Extensions
open ethCommonDB.models

let mapDecimals (EthTokenInfos: TokenInfo seq) (response: responseEthCall) = 
    let res = EthTokenInfos.Where(fun x -> x.AddressToken = response.id.ToLowerInvariant()).FirstOrDefault()

    res.Decimals <- response.result.ToInt()

    res

let mapToken0 (response: responseEthCall) = 
    let res = new TokenInfo()

    res.AddressPair <- response.id.ToLowerInvariant()
    res.AddressToken0 <- response.result.Replace("0x000000000000000000000000", "0x")

    res
    
let mapToken1 (EthTokenInfos: TokenInfo[]) (response: responseEthCall) = 
    let res = EthTokenInfos.Where(fun x -> x.AddressPair = response.id.ToLowerInvariant()).FirstOrDefault()
    res.AddressToken1 <- response.result.Replace("0x000000000000000000000000", "0x")

    res 
    
let mapToken01toAddress excludedAddresses (response: TokenInfo) = 
    
    if not (excludedAddresses |> Array.exists (fun x -> String.Equals(x, response.AddressToken0, StringComparison.InvariantCultureIgnoreCase) ) )
    then
        response.AddressToken <- response.AddressToken0
    elif not (excludedAddresses |> Array.exists (fun x -> String.Equals(x, response.AddressToken1, StringComparison.InvariantCultureIgnoreCase) ) )
    then 
        response.AddressToken <- response.AddressToken1
    else
        response.AddressToken <- ""

    response
    
