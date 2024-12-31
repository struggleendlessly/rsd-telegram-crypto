module mapResponseSwap

open System
open dbMigration.models
open responseSwap
open Extensions
open data

let EthAddress = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2".ToLowerInvariant()
let daiAddress = "0xa478c2975ab1ea89e8196811f51a7b7ade33eb11".ToLowerInvariant()

let map (responseSwapDTO: responseSwap) = 
    let res = new EthSwapsETH_USD()
    let va = responseSwapDTO.result[0]
    let data = splitString va.data [| 32; 32; 32; 32; |] // swap
    let comparer (x: string) (y: string) = StringComparer.OrdinalIgnoreCase.Compare(x, y)
    let firstInOrder = [|EthAddress; daiAddress|] |> Array.sortWith comparer

    let (EthIn, EthOut, TokenIn, TokenOut) = inOut firstInOrder data
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

