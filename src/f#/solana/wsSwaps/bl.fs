module bl

type SwapToken = 
    { 
    mutable tokenAddress: string
    mutable t0addr: string 
    mutable t1addr: string 
    mutable from: string 
    mutable to_: string 
    mutable t0amountFloat: float 
    mutable t1amountFloat: float 
    mutable t0amountInt: uint64 
    mutable t1amountInt: uint64 
    mutable t0decimals: uint64 
    mutable t1decimals: uint64 
    mutable priceTokenInSol: float 
    mutable priceSolInUsd: float 
    mutable isBuyToken: bool
    mutable isBuySol: bool
    mutable slotNuber: uint64
    }

type tokensTypes = 
    | Token of SwapToken
    | StableCoin of SwapToken
         
let emptySwapTokens = { 
    tokenAddress = ""
    t0addr = ""
    t1addr = ""
    from = ""
    to_ = ""
    t0amountFloat = 0.0
    t1amountFloat = 0.0
    t0amountInt = 0UL
    t1amountInt = 0UL
    t0decimals = 0UL
    t1decimals = 0UL
    priceTokenInSol = 0.0 
    priceSolInUsd = 0.0
    isBuyToken = false
    isBuySol = false
    slotNuber = 0UL
    }