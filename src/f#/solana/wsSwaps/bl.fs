module bl

type SwapToken = 
    { 
        tokenAddress: string
        t0addr: string 
        t1addr: string 
        from: string 
        to_: string 
        txn: string 
        t0amountFloat: float 
        t1amountFloat: float 
        t0amountInt: uint64 
        t1amountInt: uint64 
        t0decimals: uint64 
        t1decimals: uint64 
        priceTokenInSol: float 
        priceSolInUsd: float 
        isBuyToken: bool
        isBuySol: bool
        solIn: float
        solOut: float
        tokenIn: float
        tokenOut: float
    }

type instructionToken = 
    { 
     address: string 
     from: string 
     to_: string 
     txn: string 
     amountFloat: float 
     amountInt: uint64 
     decimals: uint64 
    }

type tokensTypes = 
    | TokenSol of SwapToken
    | TokenUSD of SwapToken
    | StableCoin of SwapToken
         
let emptySwapTokens = { 
    tokenAddress = ""
    t0addr = ""
    t1addr = ""
    from = ""
    to_ = ""
    txn = ""
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
    solIn = 0.0
    solOut = 0.0
    tokenIn = 0.0
    tokenOut = 0.0
    }