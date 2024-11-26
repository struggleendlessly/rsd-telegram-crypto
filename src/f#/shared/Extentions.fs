module Extensions
open System

    type System.Int32 with
        member this.ToHex() = sprintf "0x%X" this

    type System.String with
        member this.ToInt64() = Convert.ToInt64 this

    type System.String with
        member this.ToInt() = Convert.ToInt32(this, 16)