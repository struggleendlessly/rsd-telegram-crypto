module UlrBuilderSOL

open System.Text.Json
open requestSingleDTO
open Extensions

let getSlotLeader () =
     let res = 
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "getSlot"; 
                    _params = [| 
                                
                              |]; 
                    id = "1"
             } 

     //let res = JsonSerializer.Serialize request
    
     res

let getBlock 
        slot
     =
     let res = 
             { 
                 requestSingleDTO.Default 
                 with 
                    method = "getBlock"; 
                    _params = [| 
                                   slot
                                   {
                                        encoding =  "jsonParsed"
                                        maxSupportedTransactionVersion = 0
                                        transactionDetails = "full"
                                        rewards = false
                                   }                                  
                              |]; 
                    id = "1"
             } 

     //let res = JsonSerializer.Serialize request
    
     res