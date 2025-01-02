module Extensions
open System

    type System.Int32 with
        member this.ToHex() = sprintf "0x%X" this

    type System.String with
        member this.ToInt64() = Convert.ToInt64 this

    type System.String with
        member this.ToInt() =
            try
                Convert.ToInt32(this, 16)
            with
            | :? System.FormatException
            | :? System.OverflowException
            | :? ArgumentException -> -1

    type System.String with
        member this.HexToInt64() = Convert.ToInt64(this, 16)

    type Async with
        static member Bind (f: 'T -> Async<'U>) (asyncValue: Async<'T>) : Async<'U> =
            async {
                let! result = asyncValue
                return! f result
            }
