module mapGetSwaps

open dbMigration.models
open bl
open responseGetSlots
open System

let mapToSwapTokensEntity startSlot endSlot priceSolInUsd (v: SwapToken) = 
    let res = new swapsTokens()

    res.addressToken <- v.tokenAddress
    res.slotNumberStartInt <- startSlot
    res.slotNumberStartInt <- endSlot

    res.from <- v.from
    res.``to`` <- v.to_

    res.isBuySol <- v.isBuySol
    res.isBuyToken <- v.isBuyToken
    res.priceSol_USD <- priceSolInUsd
  
    res.t0amount <- v.t0amountFloat
    res.t1amount <- v.t1amountFloat

    res  

let mapToSwapTokensUSDEntity (v: SwapToken) = 
    let res = new swapsTokensUSD()

    res.addressToken <- v.tokenAddress
    res.slotNumberInt <- v.slotNuber

    res.from <- v.from
    res.``to`` <- v.to_

    res.isBuySol <- v.isBuySol
    res.isBuyDai <- v.isBuyToken
    res.priceSolInUsd <- v.priceSolInUsd
  
    res.t0amount <- v.t0amountFloat
    res.t1amount <- v.t1amountFloat

    res   


let swapsUsdToSol priceSolInUsd (stableCoins) (v: SwapToken) = 
    if stableCoins |> Array.exists (fun item -> String.Equals(item, v.t0addr, StringComparison.InvariantCultureIgnoreCase))
    then
        v.t0addr <- "So11111111111111111111111111111111111111112"
        v.t0amountFloat <- v.t0amountFloat / priceSolInUsd
        v.priceTokenInSol <- v.t0amountFloat / v.t1amountFloat
    else
        v.t1addr <- "So11111111111111111111111111111111111111112"
        v.t1amountFloat <- v.t1amountFloat / priceSolInUsd
        v.priceTokenInSol <- v.t1amountFloat / v.t0amountFloat
    v

let swapsAveragePrice (v: SwapToken) = 

    1

let mapSwapTokens stableCoins defaultSolUsd startSlot endSlot (v: tokensTypes option[])= 

    let tokensT0, others0 = 
        v 
        |> Array.partition (function 
                            | Some ( TokenSol _) -> true 
                            |  _ -> false
                            )
    let tokensUsdT0, others1 = 
        others0
        |> Array.partition (function 
                            | Some ( TokenUSD _) -> true 
                            |  _ -> false)

    let stableCoinsT0 =  
        others1 |> Array.choose (function 
                            |  Some ( StableCoin x) -> Some x 
                            | _ -> None)

    let tokensT1 = 
        tokensT0 
        |> Array.choose (function 
                            |  Some ( TokenSol x) -> Some x 
                            | _ -> None)

    let tokensUsdT1 = 
        tokensUsdT0 
        |> Array.choose (function 
                            |  Some ( TokenUSD x) -> Some x 
                            | _ -> None)
    
    let priceSolInUsd = 
        match stableCoinsT0 with
        | [||] -> defaultSolUsd
        | _ -> stableCoinsT0.[0].priceSolInUsd


    let tokensUsdT2 = tokensUsdT1
                             |> Array.map (swapsUsdToSol priceSolInUsd stableCoins)
    //let tokensT2 = tokensT1
    //                         |> Array.map (mapToSwapTokensEntity startSlot endSlot priceSolInUsd)
        
    tokensT0