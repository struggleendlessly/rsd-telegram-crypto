module responseTokenMetadata

open responseError

type responseResult = {
    decimals: int option
    logo: string option
    name: string
    symbol: string
}

type responseTokenMetadata = {
    jsonrpc: string
    id: string
    result: responseResult
    error: responseError option
}



