module mapResponseSwap

open System
open System.Linq
open ethCommonDB.models

open Extensions
open data
open responseSwap

let mapETH_USD 
        addressChainCoin 
        addressStableCoinsToInteract
        addressChainCoinDecimals 
        (t0: string)
        (t1: string) 
        (responseSwapDTO: responseSwap) = 

    let res = new SwapsETH_USD()

    if Array.isEmpty responseSwapDTO.result
    then 
        res.blockNumberInt <- responseSwapDTO.id
        Some res
    else        
        let va = responseSwapDTO.result[0]
        let data = splitString va.data [| 32; 32; 32; 32; |] // swap
        
        let firstInOrder = [|t0.ToLowerInvariant(); t1.ToLowerInvariant()|] 
                            |> Array.sortWith comparer

        let ethOrStableCoin  = TOT1_toEthOrStableCoin 
                                     addressChainCoin 
                                     addressStableCoinsToInteract 
                                     addressChainCoinDecimals 
                                     0.0
                                     firstInOrder
        let inOutOption = inOut ethOrStableCoin data

        match inOutOption with
        | Some x -> 
                    let (EthIn, EthOut, TokenIn, TokenOut) = x
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

                    Some res

        | None -> None


let mapResponseSwapResult 
        addressChainCoin
        addressStableCoinsToInteract
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

    let firstInOrder = [|token0and1.AddressToken0.ToLowerInvariant(); token0and1.AddressToken1.ToLowerInvariant()|] 
                        |> Array.sortWith comparer

    let inOutSumOption = inOutSum 
                                addressChainCoin 
                                addressStableCoinsToInteract 
                                decimals 
                                firstInOrder 
                                datas
                                ethPriceInCloseBlock

    match inOutSumOption with
    | Some (EthIn, EthOut, TokenIn, TokenOut) ->
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

            res.EthIn <- EthIn.ToString15()
            res.EthOut <- EthOut.ToString15()
            res.TokenIn <- TokenIn.ToString15()
            res.TokenOut <- TokenOut.ToString15()
    
            res.priceETH_USD <- ethPriceInCloseBlock

            if EthIn > 0 && TokenOut > 0 then
                res.priceTokenInETH <- Math.Round (decimal (EthIn / TokenOut), 20)
                res.isBuyToken <- true 

            if EthOut > 0 && TokenIn > 0 then
                res.priceTokenInETH <- Math.Round (decimal (EthOut / TokenIn ), 20)
                res.isBuyEth <- true

            if res.priceTokenInETH > 1M // it could be BITcoin or scam, we dont need them
            then 
                None
            else
                Some (res)
    | None -> None


