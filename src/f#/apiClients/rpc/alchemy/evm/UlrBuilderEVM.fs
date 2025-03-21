module UlrBuilderEVM

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

        let lastBlock = blockNumber + chainBlockInMinutes
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

let getEthCall_decimals
        ethCall_decimals 
        address =

        let res =
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "eth_call"
                    id = address
                    _params = [|
                                { 
                                    ``to`` = address
                                    data = ethCall_decimals
                                }
                              |]
             } 

     //let res = JsonSerializer.Serialize request
    
        res

let getEthCall_token0
        ethCall_token0 
        address =

        let res =
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "eth_call"
                    id = address
                    _params = [|
                                { 
                                    ``to`` = address
                                    data = ethCall_token0
                                }
                              |]
             } 

     //let res = JsonSerializer.Serialize request
    
        res

let getEthCall_token1
        ethCall_token1 
        address =

        let res =
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "eth_call"
                    id = address
                    _params = [|
                                { 
                                    ``to`` = address
                                    data = ethCall_token1
                                }
                              |]
             } 

     //let res = JsonSerializer.Serialize request
    
        res

let eth_getTransactionReceipt
        (address, trxHash) =

        let res =
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "eth_getTransactionReceipt"
                    id = address
                    _params = [|                                
                                    trxHash                               
                              |]
             } 

     //let res = JsonSerializer.Serialize request
    
        res

let getTotalSupply
        addressMethod 
        addressToken 
        =

        let res =
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "eth_call"
                    id = addressToken
                    _params = [|
                                { 
                                    ``to`` = addressToken
                                    data = addressMethod
                                }
                              |]
             } 

     //let res = JsonSerializer.Serialize request
    
        res

let getTokenNames
        addressToken 
        =

        let res =
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "alchemy_getTokenMetadata"
                    id = addressToken
                    _params = [|                               
                                    addressToken                                
                              |]
             } 

     //let res = JsonSerializer.Serialize request
    
        res