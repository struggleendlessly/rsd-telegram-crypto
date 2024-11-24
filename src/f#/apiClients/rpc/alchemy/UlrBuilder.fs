module UlrBuilder

open System.Text.Json
open requestSingleDTO
open Extensions

let getBlockByNumber (blockNumber: int) =
     let request = 
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
    
     request

let getLastBlockNumber _ =
     let request = 
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "eth_blockNumber"; 
             } 

     //let res = JsonSerializer.Serialize request
    
     request