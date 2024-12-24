module responseSwap

open responseError

type Result = {
    address: string
    blockHash: string
    blockNumber: string
    data: string
    logIndex: string
    removed: bool
    topics: string[]
    transactionHash: string
    transactionIndex: string
}

type responseSwap = {
    jsonrpc: string
    id: int
    result: Result[]
    error: responseError option
}

//{
//  "id": 1,
//  "jsonrpc": "2.0",
//  "method": "eth_getLogs",
//  "params": [
//    {
//      "address": [
//        "0xa478c2975ab1ea89e8196811f51a7b7ade33eb11"
//      ],
//      "fromBlock": "0x1461005",
//      "toBlock": "0x1462010",
//      "topics": [
//        "0xd78ad95fa46c994b6551d0da85fc275fe613ce37657fb8d5e3d130840159d822"
//      ]
//    }
//  ]
//}
