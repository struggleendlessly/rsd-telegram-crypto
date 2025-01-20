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

    match response.error with
    |None ->
        res.AddressPair <- response.id.ToLowerInvariant()
        res.AddressToken0 <- response.result.Replace("0x000000000000000000000000", "0x")
        Some res
    |Some error ->
        None   
    
let mapToken1 (EthTokenInfos: TokenInfo option []) (response: responseEthCall) = 
    match response.error with
    |None ->
        let res = 
            EthTokenInfos 
            |> Array.choose(fun x -> 
                match x with
                | Some tokenInfo -> 
                    if tokenInfo.AddressPair.ToLowerInvariant() = response.id.ToLowerInvariant() 
                    then Some tokenInfo 
                    else None
                | None -> None)
            |> Array.tryHead

        match res with
        |Some res ->
            res.AddressToken1 <- response.result.Replace("0x000000000000000000000000", "0x")
            Some res
        |None -> None

    |Some error ->
        None 

let mapToken01toAddress (excludedAddresses:string []) (response: TokenInfo option) : TokenInfo option =
    match response with
    | Some tokenInfo ->
        let addressToken0 = if not (excludedAddresses |> Array.exists (fun x -> x.CompareCI tokenInfo.AddressToken0)) 
                            then tokenInfo.AddressToken0 
                            else ""

        let addressToken1 = if not (excludedAddresses |> Array.exists (fun x -> x.CompareCI tokenInfo.AddressToken1))
                            then tokenInfo.AddressToken1 
                            else ""
        
        if addressToken0 <> "" && addressToken1 = ""
        then 
            tokenInfo.AddressToken <- addressToken0
            Some tokenInfo
        elif addressToken0 = "" && addressToken1 <> "" 
        then
            tokenInfo.AddressToken <- addressToken1
            Some tokenInfo
        else
            None
    | None -> None
    