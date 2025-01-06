module createSeq

let getSeqToProcess1 n start end1 = 
    async{
        let! startAsync  = start
        let! endAsync = end1
            
        if endAsync - startAsync > n
        then
            return seq { startAsync + 1 .. startAsync + 1000 } |> Seq.toArray
        else 
            return seq { startAsync + 1 .. endAsync } |> Seq.toArray
    }

let getSeqToProcess n step start end1 =
    async{
        let! startAsync  = start()
        let! endAsync = end1()
            
        if endAsync - startAsync > n
        then
            return seq { startAsync + 1 .. step .. startAsync + n } |> Seq.toArray
        elif endAsync - startAsync > step
        then
            return seq { startAsync + 1 .. step .. endAsync } |> Seq.toArray
        else
            return [||]
    }

let getSeqToProcessUint64 n step (start: unit -> Async<uint64>) (end1:unit -> Async<uint64>) =
    async{
        let! startAsync  = start()
        let! endAsync = end1()
        let diff = endAsync - startAsync
        if endAsync - startAsync > n
        then
            return seq { startAsync + 1UL  .. startAsync + n } |> Seq.toArray
        elif endAsync - startAsync > step
        then
            return seq { startAsync + 1UL  .. endAsync } |> Seq.toArray
        else
            return [||]
    }