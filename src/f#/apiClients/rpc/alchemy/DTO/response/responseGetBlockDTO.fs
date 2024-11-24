module responseGetBlockDTO

open System.Collections.Generic

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

type responseGetBlockDTO = {
    jsonrpc: string
    id: int
    result: Result
}

type responseGetBlocksDTO = responseGetBlockDTO[]
