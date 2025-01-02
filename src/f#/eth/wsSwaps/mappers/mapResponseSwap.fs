module mapResponseSwap

open System
open dbMigration.models
open responseSwap
open Extensions
open data

let map (responseSwapDTO: responseSwap) = 
    let res = new EthSwapsETH_USD()

    if Array.isEmpty responseSwapDTO.result
    then 
        res.blockNumberInt <- responseSwapDTO.id
    else        
        let va = responseSwapDTO.result[0]
        let data = splitString va.data [| 32; 32; 32; 32; |] // swap
        
        let firstInOrder = [|ethStrings.addressETH; ethStrings.addressDai|] |> Array.sortWith ethStrings.comparer

        let (EthIn, EthOut, TokenIn, TokenOut) = inOut 18 firstInOrder data
        res.blockNumberInt <- responseSwapDTO.id
        res.pairAddress <- va.address

        res.txsHash <- va.transactionHash
        res.from <- va.topics[1]   
        res.``to`` <- va.topics[2]

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


let mapResponseSwapResult blockId decimals ethPriceInCloseBlock (address:string , responseSwapDTO: responseSwap.Result []) = 
    
    let res = new EthSwapsETH_Token()
    let datas =
        responseSwapDTO
        |> Array.map (fun x -> splitString x.data [| 32; 32; 32; 32; |])

    let firstInOrder = [|ethStrings.addressETH; address.ToLowerInvariant()|] |> Array.sortWith ethStrings.comparer
    let (EthIn, EthOut, TokenIn, TokenOut) = inOutAvarage decimals firstInOrder datas 


    res.blockNumberStartInt <- blockId - ethStrings.ethChainBlocksIn5Minutes
    res.blockNumberEndInt <- blockId
    
    res.pairAddress <- address

    let (from1, to1) = responseSwapDTO 
                    |> Array.map (fun result -> result.topics)
                    |> getFromTo

    //res.txsHash <- va.transactionHash

    res.from <- from1   
    res.``to`` <- to1

    res.EthIn <- EthIn.ToString()
    res.EthOut <- EthOut.ToString()
    res.TokenIn <- TokenIn.ToString()
    res.TokenOut <- TokenOut.ToString()
    res.priceETH_USD <- ethPriceInCloseBlock

    if EthIn > 0 && TokenOut > 0 then
        res.priceTokenInETH <- Math.Round (float (EthIn / TokenOut), 10)
        res.isBuyToken <- true 

    if EthOut > 0 && TokenIn > 0 then
        res.priceTokenInETH <- Math.Round (float (EthOut / TokenIn ), 10)
        res.isBuyEth <- true

    res


