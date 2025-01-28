module responseEthCall

open responseError

type responseEthCall = {
    jsonrpc: string
    id: string
    result: string
    error: responseError option
}

type responseEthCalls = {
    responseEthCalls: responseEthCall seq
}

