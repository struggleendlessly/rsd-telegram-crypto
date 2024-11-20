module UlrBuilder

open System.Text.Json
open requestBaseDTO
open Extensions

let getBlockByNumber (blockNumber: int) =
     let request = 
             { 
                 requestBaseDTO.Default 
                 with 
                    method = "eth_getBlockByNumber"; 
                    _params = [| 
                                blockNumber.ToHex(); 
                                true 
                              |]; 
                    id = blockNumber
             } 

     let res = JsonSerializer.Serialize(request)
    
     res