module eee

open System.Text.Json
open responseGetBlockDTO


let prepareChunks1<'T> : int -> 'T=                
        JsonSerializer.Deserialize<'T>

let aa = prepareChunks1<responseGetBlockDTO> 1