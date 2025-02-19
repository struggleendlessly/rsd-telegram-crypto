module Extensions
open System
open System.Text.RegularExpressions
open Nethereum.Util

    type System.Int32 with
        member this.ToHex() = sprintf "0x%X" this

    type System.String with
        member this.ToZero() = if String.IsNullOrEmpty this then "0" else this

    type System.String with
        member this.ToInt64() = Convert.ToInt64 this
    
    type BigDecimal with
        member this.ToString15() =
            let regex = Regex(@"^\d+(\.\d{0,15})?")
            let matchResult = regex.Match(this.ToString())
            
            if matchResult.Success && 
               matchResult.Value <> "0" 
            then matchResult.Value 
            else ""

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

    type System.String with
       member s1.CompareCI(s2: string) =
         System.String.Equals(s1, s2, System.StringComparison.InvariantCultureIgnoreCase)

    type Async with
        static member Bind (f: 'T -> Async<'U>) (asyncValue: Async<'T>) : Async<'U> =
            async {
                let! result = asyncValue
                return! f result
            }
