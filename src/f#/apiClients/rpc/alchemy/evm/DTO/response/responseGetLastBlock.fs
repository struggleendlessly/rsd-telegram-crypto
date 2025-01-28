module responseGetLastBlock

open responseError
open Extensions
open System

type responseGetLastBlock = {
    jsonrpc: string
    id: int
    result: string
    error: responseError option
}with 
    member this.blockInt = 
        match this.error with
        | None -> 
            match String.IsNullOrEmpty(this.result) with
            | false -> this.result.ToInt()
            | true -> 0 
        | Some _ -> 0

type responseGetLastBlocks = responseGetLastBlock seq
