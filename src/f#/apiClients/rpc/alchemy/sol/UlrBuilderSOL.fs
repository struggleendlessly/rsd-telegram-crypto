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