module UlrBuilderSOL

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