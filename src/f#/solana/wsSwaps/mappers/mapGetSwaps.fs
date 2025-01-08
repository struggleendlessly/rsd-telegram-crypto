module mapGetSwaps

open dbMigration.models
open bl
open responseGetSlots

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

    if v.isBuySol
    then       
        res.solIn <- v.t0amountFloat.ToString()
        res.solOut <- "" 
        res.tokenIn <- ""
        res.tokenOut <- v.t1amountFloat.ToString()
    else
        res.solIn <- ""
        res.solOut <- v.t0amountFloat.ToString()
        res.tokenIn <- v.t1amountFloat.ToString()
        res.tokenOut <- ""

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

    if v.isBuySol
    then       
        res.solIn <- v.t0amountFloat.ToString()
        res.solOut <- "" 
        res.tokenIn <- ""
        res.tokenOut <- v.t1amountFloat.ToString()
    else
        res.solIn <- ""
        res.solOut <- v.t0amountFloat.ToString()
        res.tokenIn <- v.t1amountFloat.ToString()
        res.tokenOut <- ""

    res   

let mapSwapTokens startSlot endSlot (v: tokensTypes option[])= 
    let tokensT0, stableCoinsT0 = 
                            v 
                            |> Array.partition (function 
                                                | Some ( Token _) -> true 
                                                | _ -> false)
    let tokensT1 = tokensT0 
                            |> Array.choose (function 
                                                |  Some ( Token x)  -> Some x 
                                                | _ -> None)
    let stableCoinsT1 = stableCoinsT0 
                            |> Array.choose (function 
                                                |  Some ( StableCoin x)  -> Some x 
                                                | _ -> None)
    //let stableCoinsT2 = stableCoinsT1
    //                         |> Array.map mapToSwapTokensUSDEntity
    //                         |> Array.fold (fun (price, count) x -> acc @ [x]) (0 ,0)

    let priceSolInUsd = stableCoinsT1[0].priceSolInUsd

    let tokensT2 = tokensT1
                             |> Array.map (mapToSwapTokensEntity startSlot endSlot priceSolInUsd)
        
    tokensT0