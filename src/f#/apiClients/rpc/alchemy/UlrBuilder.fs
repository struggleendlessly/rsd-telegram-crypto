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

let getSwapLogsUri 
        pairAddress 
        topic 
        (blockNumber: int) =

        let res =
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "eth_getLogs"
                    id = blockNumber
                    _params = [|
                                { 
                                    topics = [|topic|] 
                                    address = [|pairAddress|]
                                    fromBlock = blockNumber.ToHex()
                                    toBlock = blockNumber.ToHex()
                                }
                              |]
             } 

     //let res = JsonSerializer.Serialize request
    
        res