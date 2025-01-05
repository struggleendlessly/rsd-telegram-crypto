module responseGetTransactionReceipt

open responseError

type Log = {
    blockHash: string
    address: string
    logIndex: string
    data: string
    removed: bool
    topics: string[]
    blockNumber: string
    transactionIndex: string
    transactionHash: string
}

type Result = {
    transactionHash: string
    blockHash: string
    blockNumber: string
    logsBloom: string
    gasUsed: string
    contractAddress: obj
    cumulativeGasUsed: string
    transactionIndex: string
    from: string
    ``to``: string
    ``type``: string
    effectiveGasPrice: string
    logs: Log[]
    status: string
}

type responseGetTransactionReceipt = {
    jsonrpc: string
    id: string
    result: Result
}

