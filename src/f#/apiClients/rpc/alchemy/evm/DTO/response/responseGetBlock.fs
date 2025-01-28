module responseGetBlock

open System.Collections.Generic
open responseError

type AccessList = {
    address: string
    storageKeys: List<string>
}

type Transaction = {
    blockHash: string
    blockNumber: string
    hash: string
    yParity: string
    accessList: List<AccessList>
    transactionIndex: string
    ``type``: string
    nonce: string
    input: string
    r: string
    s: string
    chainId: string
    v: string
    gas: string
    maxPriorityFeePerGas: string
    from: string
    ``to``: string
    maxFeePerGas: string
    value: string
    gasPrice: string
}

type Withdrawal = {
    amount: string
    address: string
    index: string
    validatorIndex: string
}

type Result = {
    number: string
    hash: string
    transactions: List<Transaction>
    logsBloom: string
    totalDifficulty: string
    receiptsRoot: string
    extraData: string
    withdrawalsRoot: string
    baseFeePerGas: string
    nonce: string
    miner: string
    withdrawals: List<Withdrawal>
    excessBlobGas: string
    difficulty: string
    gasLimit: string
    gasUsed: string
    uncles: List<obj>
    parentBeaconBlockRoot: string
    size: string
    sha3Uncles: string
    transactionsRoot: string
    stateRoot: string
    mixHash: string
    parentHash: string
    blobGasUsed: string
    timestamp: string
}

type responseGetBlock = {
    jsonrpc: string
    id: int
    result: Result
    error: responseError
}

type responseGetBlocks = responseGetBlock seq
