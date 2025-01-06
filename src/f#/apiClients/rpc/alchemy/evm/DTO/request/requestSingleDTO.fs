module requestSingleDTO

open System.Text.Json.Serialization

type requestSingleDTO = {
    jsonrpc: string
    method: string
    [<JsonPropertyName("params")>]
    _params: obj[]
    id: obj
}
with
    static member Default = 
         {
              jsonrpc = "2.0"
              method = "eth_getBlockByNumber"
              _params = [||]
              id = 0
          }

type requestSwapDto = {
    topics: string[]
    address: string[]
    fromBlock: string
    toBlock: string
}

type requestSwapDto_NoAddress = {
    topics: string[]
    fromBlock: string
    toBlock: string
}

type requestEthCallDto_decimals = {
    ``to``: string
    data: string
}

type requestSol_GetBlock = {
    encoding: string
    maxSupportedTransactionVersion: int
    transactionDetails: string
    rewards: bool
}
