module mapResponseSwap

open System
open System.Linq
open System.Text.RegularExpressions

open ethCommonDB.models

open data
open responseSwap

let mapETH_USD 
        addressChainCoin 
        addressChainCoinDecimals 
        t0 
        t1 
        (responseSwapDTO: responseSwap) = 

    let res = new SwapsETH_USD()

    if Array.isEmpty responseSwapDTO.result
    then 
        res.blockNumberInt <- responseSwapDTO.id
    else        
        let va = responseSwapDTO.result[0]
        let data = splitString va.data [| 32; 32; 32; 32; |] // swap
        
        let firstInOrder = [|t0; t1|] 
                            |> Array.sortWith comparer

        let (EthIn, EthOut, TokenIn, TokenOut) = inOut addressChainCoin addressChainCoinDecimals firstInOrder data
        res.blockNumberInt <- responseSwapDTO.id
        res.pairAddress <- va.address

        res.txsHash <- va.transactionHash
        res.from <- va.topics[1].Replace("000000000000000000000000", "")  
        res.``to`` <- va.topics[2].Replace("000000000000000000000000", "")

        res.EthIn <- EthIn.ToString()
        res.EthOut <- EthOut.ToString()
        res.TokenIn <- TokenIn.ToString()

        res.TokenOut <- TokenOut.ToString()

        if EthIn > 0 && TokenOut > 0 then
            res.priceEthInUsd <- Math.Round (float (TokenOut / EthIn), 2)
            res.isBuyDai <- true 

        if EthOut > 0 && TokenIn > 0 then
            res.priceEthInUsd <- Math.Round (float (TokenIn / EthOut), 2)
            res.isBuyEth <- true

    res


let mapResponseSwapResult 
        addressChainCoin
        blocksIn5Minutes
        blockId 
        (token0and1: TokenInfo seq)  
        decimals 
        ethPriceInCloseBlock 
        (address:string , responseSwapDTO: responseSwap.Result []) = 
    
    let res = new SwapsETH_Token()
    let token0and1 = token0and1.Where(fun x -> x.AddressPair = address.ToLowerInvariant()).FirstOrDefault()

    let datas =
        responseSwapDTO
        |> Array.map (fun x -> splitString x.data [| 32; 32; 32; 32; |])

    let firstInOrder = [|token0and1.AddressToken0; token0and1.AddressToken1|] 
                        |> Array.sortWith comparer

    let (EthIn, EthOut, TokenIn, TokenOut) = inOutSum addressChainCoin decimals firstInOrder datas 

    res.blockNumberStartInt <- blockId - blocksIn5Minutes
    res.blockNumberEndInt <- blockId
    
    res.pairAddress <- address

    let (from1, to1) = responseSwapDTO 
                    |> Array.map (fun result -> result.topics)
                    |> getFromTo
    let hashes = responseSwapDTO 
                    |> Array.map (fun result -> result.transactionHash)
                    |> String.concat ", "

    res.txsHash <- hashes

    res.from <- from1   
    res.``to`` <- to1

    // Regex to capture up to 5 decimal places
    let regex = Regex(@"^\d+\.\d{0,7}")

    res.EthIn <- regex.Match(EthIn.ToString()).ToString()
    res.EthOut <- regex.Match(EthOut.ToString()).ToString() 
    res.TokenIn <-regex.Match(TokenIn.ToString()).ToString() 
    res.TokenOut <- regex.Match(TokenOut.ToString()).ToString()
    
    res.priceETH_USD <- ethPriceInCloseBlock

    if EthIn > 0 && TokenOut > 0 then
        res.priceTokenInETH <- Math.Round (float (EthIn / TokenOut), 10)
        res.isBuyToken <- true 

    if EthOut > 0 && TokenIn > 0 then
        res.priceTokenInETH <- Math.Round (float (EthOut / TokenIn ), 10)
        res.isBuyEth <- true

    res


