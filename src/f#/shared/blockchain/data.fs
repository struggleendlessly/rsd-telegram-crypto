module data

open Extensions
open System.Numerics
open Nethereum.Util
open System
open System.Globalization

let comparer (x: string) (y: string) = StringComparer.OrdinalIgnoreCase.Compare(x, y)

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

type TOT1_inOutIndexAndDecimals =
    {
        ethIndex1: int
        ethIndex2: int
        tokenIndex1: int
        tokenIndex2: int

        ethDecimals: int
        tokenDecimals: int

        ethUsdPrice: float
    }

type TOT1_toEthOrStableCoin = 
    | Eth of TOT1_inOutIndexAndDecimals
    | StableCoin of TOT1_inOutIndexAndDecimals

let TOT1_v1 = 
    {
        ethIndex1 = 0 
        ethIndex2 = 2
        tokenIndex1 = 1 
        tokenIndex2 = 3 
        ethDecimals = 18
        tokenDecimals = 6
        ethUsdPrice = 0 }

let TOT1_v2 =
    {
        ethIndex1 = 1 
        ethIndex2 = 3
        tokenIndex1 = 0 
        tokenIndex2 = 2 
        ethDecimals = 18
        tokenDecimals = 6
        ethUsdPrice = 0 }

let TOT1_toEthOrStableCoin 
        (addressChainCoin: string) 
        (addressStableCoinsToInteract: string []) 
        (decimals: int)
        (ethUsdPrice: float)
        (listT0T1: string []) = 

        if Array.contains listT0T1[0] addressStableCoinsToInteract && not (Array.contains listT0T1[1] addressStableCoinsToInteract) 
        then
            Some ( StableCoin  { TOT1_v1 with
                                    ethDecimals = 6
                                    tokenDecimals = decimals
                                    ethUsdPrice = ethUsdPrice })
        elif Array.contains listT0T1[1] addressStableCoinsToInteract && not (Array.contains listT0T1[0] addressStableCoinsToInteract)
        then
            Some (StableCoin { TOT1_v2 with
                                    ethDecimals = 6
                                    tokenDecimals = decimals
                                    ethUsdPrice = ethUsdPrice })
        elif addressChainCoin.CompareCI(listT0T1[0]) && not (addressChainCoin.CompareCI(listT0T1[1]))
        then
            Some (Eth { TOT1_v1 with
                                    ethDecimals = 18
                                    tokenDecimals = decimals
                                    ethUsdPrice = ethUsdPrice })
        elif addressChainCoin.CompareCI(listT0T1[1]) && not (addressChainCoin.CompareCI(listT0T1[0]))
        then
            Some (Eth { TOT1_v2 with
                                    ethDecimals = 18
                                    tokenDecimals = decimals
                                    ethUsdPrice = ethUsdPrice })
        else    
            None

let inOut 
        (ethOrStableCoin: TOT1_toEthOrStableCoin option)
        (listValues: BigInteger list) =
    
    match ethOrStableCoin with
    | Some (StableCoin x) -> 
                        let EthIn = (toBigDecimalsWithPrecision x.ethDecimals listValues.[x.ethIndex1] / BigDecimal.Parse(x.ethUsdPrice|> string))
                        let EthOut = (toBigDecimalsWithPrecision x.ethDecimals listValues.[x.ethIndex2] / BigDecimal.Parse(x.ethUsdPrice|> string))
                        let TokenIn = toBigDecimalsWithPrecision x.tokenDecimals listValues.[x.tokenIndex1]
                        let TokenOut = toBigDecimalsWithPrecision x.tokenDecimals listValues.[x.tokenIndex2]

                        Some (EthIn, EthOut, TokenIn, TokenOut)
    | Some (Eth x) -> 
                        let EthIn = toBigDecimalsWithPrecision x.ethDecimals listValues.[x.ethIndex1]
                        let EthOut = toBigDecimalsWithPrecision x.ethDecimals listValues.[x.ethIndex2]
                        let TokenIn = toBigDecimalsWithPrecision x.tokenDecimals listValues.[x.tokenIndex1]
                        let TokenOut = toBigDecimalsWithPrecision x.tokenDecimals listValues.[x.tokenIndex2]

                        Some (EthIn, EthOut, TokenIn, TokenOut)
    | None -> None

    //let EthIn = toBigDecimalsWithPrecision TOT1_toEthOrStableCoin.ethDecimals listValues.[TOT1_toEthOrStableCoin.ethIndex1]
    //let EthOut = toBigDecimalsWithPrecision TOT1_toEthOrStableCoin.ethDecimals listValues.[TOT1_toEthOrStableCoin.ethIndex2]
    //let TokenIn = toBigDecimalsWithPrecision TOT1_toEthOrStableCoin.tokenDecimals listValues.[TOT1_toEthOrStableCoin.tokenIndex1]
    //let TokenOut = toBigDecimalsWithPrecision TOT1_toEthOrStableCoin.tokenDecimals listValues.[TOT1_toEthOrStableCoin.tokenIndex2]

    //EthIn, EthOut, TokenIn, TokenOut

let inOutSum 
        addressChainCoin 
        addressStableCoinsToInteract 
        decimals 
        (listT0T1: string []) 
        (listValues: BigInteger list seq) 
        ethPriceInCloseBlock
        =

    let sumBigDecimals (a: BigDecimal) (b: BigDecimal) = a + b
    let divideBigDecimal (a: BigDecimal) (b: BigDecimal) = a / b

    let ethInSum, ethOutSum, tokenInSum, tokenOutSum =
        listValues
        |> Seq.map ( fun x -> 
                          let ethOrStableCoin = TOT1_toEthOrStableCoin 
                                                             addressChainCoin 
                                                             addressStableCoinsToInteract 
                                                             decimals 
                                                             ethPriceInCloseBlock
                                                             listT0T1
                          let inOutOption = inOut ethOrStableCoin x
                          let aa = None
                          inOutOption
                          )
        |> Seq.choose id
        |> Seq.fold (fun (ethInAcc, ethOutAcc, tokenInAcc, tokenOutAcc) 
                           (ethIn, ethOut, tokenIn, tokenOut) ->
                                (sumBigDecimals ethInAcc ethIn, 
                                 sumBigDecimals ethOutAcc ethOut, 
                                 sumBigDecimals tokenInAcc tokenIn, 
                                 sumBigDecimals tokenOutAcc tokenOut)
                       ) (BigDecimal(), BigDecimal(), BigDecimal(), BigDecimal())

    let count = BigDecimal.Parse(listValues |> Seq.length |> string)

    if ethInSum = BigDecimal.Parse("0") && 
       tokenOutSum = BigDecimal.Parse("0") &&
       ethOutSum = BigDecimal.Parse("0") &&
       tokenInSum = BigDecimal.Parse("0")
    then
        None
    else
        Some (ethInSum, ethOutSum, tokenInSum, tokenOutSum)

let getFromTo (topics: string[][]) =
    let (fromList, toList) =
        topics
        |> Array.fold (fun (fromAcc, toAcc) topics ->
            (topics.[1] :: fromAcc, topics.[2] :: toAcc)
        ) ([], [])
    ((String.concat ", " fromList).Replace("000000000000000000000000", ""), (String.concat ", " toList).Replace("000000000000000000000000", ""))