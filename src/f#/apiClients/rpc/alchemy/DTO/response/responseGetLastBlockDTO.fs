module responseGetLastBlockDTO

open responseErrorDTO
open Extensions
open System

type responseGetLastBlockDTO = {
    jsonrpc: string
    id: int
    result: string
    error: responseErrorDTO option
}with 
    member this.blockInt = 
        match this.error with
        | None -> 
            match String.IsNullOrEmpty(this.result) with
            | false -> this.result.ToInt()
            | true -> 0 
        | Some _ -> 0

type responseGetLastBlocksDTO = responseGetLastBlockDTO[]
