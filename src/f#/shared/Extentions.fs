module Extensions
    type System.Int32 with
        member this.ToHex() = sprintf "0x%X" this