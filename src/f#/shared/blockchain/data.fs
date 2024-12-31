module data

open System.Numerics
open Nethereum.Util
open System
open System.Globalization

let EthAddress = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2".ToLowerInvariant()

let splitString (hexString: string) (byteRanges: int []) =
    let bytes = 
        hexString.Substring(2) 
        |> Seq.chunkBySize 2 
        |> Seq.map (fun byteStr -> System.Byte.Parse(byteStr, System.Globalization.NumberStyles.HexNumber))
        |> Seq.toArray

    let splitAt (pos: int) (seq: array<'a>) =
        let a = seq |> Array.take pos
        let b = seq |> Array.skip pos
        a, b

    let initialState = ([], bytes)
    
    let result, _ = 
        byteRanges 
        |> Array.fold (fun (acc, remainingBytes) range -> 
            let part, rest = splitAt range remainingBytes
            (acc @ [part], rest)
        ) initialState
    
    let a = result |> List.map (fun x -> x |> Array.map (fun y -> y.ToString("X2")) |> String.Concat )
    let b = a|>  List.map (fun x ->  BigInteger.Parse(x, NumberStyles.AllowHexSpecifier)) 
    b
let toBigDecimalsWithPrecision precision (bigInt: BigInteger) =
    let bigIntStr = bigInt.ToString()
    let paddedStr = bigIntStr.PadLeft(precision, '0')
    let len = paddedStr.Length
    let integerPart = paddedStr.Substring(0, len - precision)
    let fractionalPart = paddedStr.Substring(len - precision)
    let combinedStr = sprintf "%s.%s" integerPart fractionalPart
    BigDecimal.Parse(combinedStr)

let inOut (listT0T1: string[]) (listValues: BigInteger list) =

    let mutable EthIn: BigDecimal= BigDecimal()
    let mutable EthOut: BigDecimal= BigDecimal()
    let mutable TokenIn: BigDecimal= BigDecimal()
    let mutable TokenOut: BigDecimal= BigDecimal()

    if String.Equals( listT0T1.[0], EthAddress, StringComparison.InvariantCultureIgnoreCase) then
        EthIn <- toBigDecimalsWithPrecision 18 listValues.[0]
        EthOut <- toBigDecimalsWithPrecision 18 listValues.[2]
        TokenIn <- toBigDecimalsWithPrecision 18 listValues.[1]
        TokenOut <- toBigDecimalsWithPrecision 18 listValues.[3]
    else
        EthIn <- toBigDecimalsWithPrecision 18 listValues.[1]
        EthOut <- toBigDecimalsWithPrecision 18  listValues.[3]
        TokenIn <- toBigDecimalsWithPrecision 18 listValues.[0]
        TokenOut <- toBigDecimalsWithPrecision 18 listValues.[2]

    EthIn, EthOut, TokenIn, TokenOut