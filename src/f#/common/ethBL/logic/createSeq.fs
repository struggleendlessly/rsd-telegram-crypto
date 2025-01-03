module createSeq

let createSeq1 n start end1 = 
    async{
        let! startAsync  = start
        let! endAsync = end1
            
        if endAsync - startAsync > n
        then
            return seq { startAsync + 1 .. startAsync + 1000 } |> Seq.toArray
        else 
            return seq { startAsync + 1 .. endAsync } |> Seq.toArray
    }