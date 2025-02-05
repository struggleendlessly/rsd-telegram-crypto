module bl_createSeq

let getSeqToProcess1 n start end1 = 
    async{
        let! startAsync  = start
        let! endAsync = end1
        let diff = endAsync - startAsync

        if endAsync - startAsync > n
        then
            return seq { startAsync + 1 .. startAsync + 1000 } 
        else 
            return seq { startAsync + 1 .. endAsync }
    }

let getSeqToProcess n step start end1 =
    async{
        let! startAsync  = start()
        let! endAsync = end1()
        let diff = endAsync - startAsync 

        if endAsync - startAsync > n
        then
            return seq { startAsync + 1 .. step .. startAsync + n } 
        elif endAsync - startAsync > step
        then
            return Seq.singleton startAsync
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
            return seq { startAsync + 1UL  .. startAsync + n } 
        elif endAsync - startAsync > step
        then
            return seq { startAsync + 1UL  .. endAsync }
        else
            return [||]
    }