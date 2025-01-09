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
let average (a: float) (b: float) = (a + b) / 2.0
let swapsUsdToSol priceSolInUsd (stableCoins) (v: SwapToken) = 
    if stableCoins |> Array.exists (fun item -> String.Equals(item, v.t0addr, StringComparison.InvariantCultureIgnoreCase))
    then
        { v with 
            t0addr = "So11111111111111111111111111111111111111112"
            t0amountFloat = v.t0amountFloat / priceSolInUsd
            priceTokenInSol = v.t0amountFloat / priceSolInUsd / v.t1amountFloat
            isBuyToken = true
            }
    else
        { v with 
            t1addr = "So11111111111111111111111111111111111111112"
            t1amountFloat = v.t1amountFloat / priceSolInUsd
            priceTokenInSol = v.t1amountFloat / priceSolInUsd / v.t0amountFloat
            isBuySol = true
            }    

let swapsAveragePrice (v: string * SwapToken[]) = 
    let f = v |> snd |> Array.head
    let acc = { f with 
                    solIn = 0 
                    solOut = 0
                    tokenIn = 0
                    tokenOut = 0
                    }
    let a = v 
            |> snd 
            |> Array.fold (fun acc x -> 

                            if x.isBuyToken 
                            then                               
                                acc.isBuyToken <- x.isBuyToken
                                acc.slotNuber <- x.slotNuber

                                acc.solIn <- average x.t0amountFloat acc.solIn
                                acc.tokenOut <- average x.t1amountFloat acc.tokenOut
                            else
                                acc.isBuySol <- x.isBuySol
                                acc.slotNuber <- x.slotNuber

                                acc.solOut <- average x.t1amountFloat acc.solOut
                                acc.tokenIn <- average x.t0amountFloat acc.tokenIn
                            acc

                            ) acc
    a
    

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
                             |> Array.filter (fun x -> x.priceTokenInSol < 1 )

    let tokensT2 = Array.append tokensUsdT2 tokensT1 
                   |> Array.filter (fun x -> x.priceTokenInSol > 0 )
                   |> Array.groupBy (fun t -> if t.t0addr <> "So11111111111111111111111111111111111111112" then t.t0addr else t.t1addr)
                   |> Array.map swapsAveragePrice
                   //|> Array.map (mapToSwapTokensEntity startSlot endSlot priceSolInUsd)
        
    tokensT0