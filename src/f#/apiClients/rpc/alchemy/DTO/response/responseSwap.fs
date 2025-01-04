module responseSwap

open responseError

type Result = {
    address: string
    blockHash: string
    blockNumber: string
    data: string
    logIndex: string
    removed: bool
    topics: string[]
    transactionHash: string
    transactionIndex: string
}

type responseSwap = {
    jsonrpc: string
    id: int
    result: Result[]
    error: responseError option
}

type responseSwaps = {
    responseSwaps: responseSwap[]
}

let filterBlocks (blocks:responseSwap[]) = 
            let filtered = blocks |> Array.filter (fun x -> not (Array.isEmpty x.result))
             
            if Array.isEmpty blocks 
            then
                    blocks             
            elif Array.isEmpty filtered
            then
                    [| blocks |> Array.maxBy (fun x -> x.id) |]
            else
                    filtered
