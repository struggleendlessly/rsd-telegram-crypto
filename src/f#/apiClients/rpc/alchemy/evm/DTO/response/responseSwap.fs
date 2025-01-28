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

let validateBlocks (blocks:responseSwap seq) = 
            let filtered = blocks |> Seq.filter (fun x -> not (Array.isEmpty x.result))
             
            if Seq.isEmpty blocks 
            then
                    blocks             
            elif Seq.isEmpty filtered
            then
                    [| blocks |> Seq.maxBy (fun x -> x.id) |]
            else
                    filtered
