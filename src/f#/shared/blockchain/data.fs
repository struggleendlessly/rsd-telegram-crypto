module data

open System.Numerics
open Nethereum.Util
open System
open System.Globalization

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
    let b = a |> List.map (fun x ->  BigInteger.Parse(x, NumberStyles.AllowHexSpecifier)) 
    b

let toBigDecimalsWithPrecision precision (bigInt: BigInteger) =
    let bigIntStr = bigInt.ToString()
    let paddedStr = bigIntStr.PadLeft(precision, '0')
    let len = paddedStr.Length
    let integerPart = paddedStr.Substring(0, len - precision)
    let fractionalPart = paddedStr.Substring(len - precision)
    let combinedStr = sprintf "%s.%s" integerPart fractionalPart
    BigDecimal.Parse(combinedStr)

let inOut decimals (listT0T1: string[]) (listValues: BigInteger list) =
    let (ethIndex1, ethIndex2, tokenIndex1, tokenIndex2) =
        if String.Equals(listT0T1.[0], ethStrings.addressETH, StringComparison.InvariantCultureIgnoreCase) then
            (0, 2, 1, 3)
        else
            (1, 3, 0, 2)
    
    let EthIn = toBigDecimalsWithPrecision decimals listValues.[ethIndex1]
    let EthOut = toBigDecimalsWithPrecision decimals listValues.[ethIndex2]
    let TokenIn = toBigDecimalsWithPrecision decimals listValues.[tokenIndex1]
    let TokenOut = toBigDecimalsWithPrecision decimals listValues.[tokenIndex2]

    EthIn, EthOut, TokenIn, TokenOut

let inOutAvarage decimals (listT0T1: string[]) (listValues: BigInteger list array) =
    let sumBigDecimals (a: BigDecimal) (b: BigDecimal) = a + b
    let divideBigDecimal (a: BigDecimal) (b: BigDecimal) = a / b

    let ethInSum, ethOutSum, tokenInSum, tokenOutSum =
        listValues
        |> Array.map (inOut decimals listT0T1)
        |> Array.fold (fun (ethInAcc, ethOutAcc, tokenInAcc, tokenOutAcc) (ethIn, ethOut, tokenIn, tokenOut) ->
            (sumBigDecimals ethInAcc ethIn, sumBigDecimals ethOutAcc ethOut, sumBigDecimals tokenInAcc tokenIn, sumBigDecimals tokenOutAcc tokenOut)
        ) (BigDecimal(), BigDecimal(), BigDecimal(), BigDecimal())

    let count = BigDecimal.Parse(listValues.Length.ToString())

    let ethInAvg = divideBigDecimal ethInSum count
    let ethOutAvg = divideBigDecimal ethOutSum count
    let tokenInAvg = divideBigDecimal tokenInSum count
    let tokenOutAvg = divideBigDecimal tokenOutSum count

    ethInAvg, ethOutAvg, tokenInAvg, tokenOutAvg

let getFromTo (topics: string[][]) =
    let (fromList, toList) =
        topics
        |> Array.fold (fun (fromAcc, toAcc) topics ->
            (topics.[1] :: fromAcc, topics.[2] :: toAcc)
        ) ([], [])
    ((String.concat ", " fromList).Replace("000000000000000000000000", ""), (String.concat ", " toList).Replace("000000000000000000000000", ""))
