module UlrBuilder

open System.Text.Json
open requestSingleDTO
open Extensions

let getBlockByNumberUri (blockNumber: int) =
     let res = 
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "eth_getBlockByNumber"; 
                    _params = [| 
                                blockNumber.ToHex(); 
                                true 
                              |]; 
                    id = blockNumber
             } 

     //let res = JsonSerializer.Serialize request
    
     res

let getLastBlockNumberUri _ =
     let res = 
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "eth_blockNumber"; 
             } 

     //let res = JsonSerializer.Serialize request
    
     res

let getSwapLogsUri_DAI 
        pairAddress 
        topic 
        (chainBlockInMinutes: int)
        (blockNumber: int) =

        let lastBlock = blockNumber + chainBlockInMinutes - 1
        let res =
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "eth_getLogs"
                    id = lastBlock
                    _params = [|
                                { 
                                    topics = [|topic|] 
                                    address = [|pairAddress|]
                                    fromBlock = blockNumber.ToHex()
                                    toBlock = lastBlock.ToHex()
                                }
                              |]
             } 

     //let res = JsonSerializer.Serialize request
    
        res

let getSwapLogsUri_Token
        topic 
        (chainBlockInMinutes: int)
        (blockNumber: int) =

        let lastBlock = blockNumber + chainBlockInMinutes - 1
        let res =
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "eth_getLogs"
                    id = lastBlock
                    _params = [|
                                { 
                                    topics = [|topic|] 
                                    fromBlock = blockNumber.ToHex()
                                    toBlock = lastBlock.ToHex()
                                }
                              |]
             } 

     //let res = JsonSerializer.Serialize request
    
        res