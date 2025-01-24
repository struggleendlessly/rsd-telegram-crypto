module bl_others

open Nethereum.Util
open System.Text.RegularExpressions
open System

let list1andLast (lst: 'a list) =
    match lst with
    | [] -> []
    | [x] -> []
    | h1 :: h2 :: h3 :: h4 :: tail -> 
        let last = List.last tail
        [h3; last]
    | head :: tail -> 
        let last = List.last tail
        [head; last]

let splitList list = 
    let half = List.length list / 2 
    let firstHalf = list |> List.take half 
    let secondHalf = list |> List.skip half 
    (firstHalf, secondHalf)

//let md5 (input : string) : string =
//    let data = Encoding.UTF8.GetBytes(input)
//    use md5 = MD5.Create()
//    (StringBuilder(), md5.ComputeHash(data))
//    ||> Array.fold (fun sb b -> sb.Append(b.ToString("x2")))
//    |> string   
let addDots (input: string) = 
    let regex = Regex(@"\B(?=(\d{3})+(?!\d))")
    regex.Replace(input, ".")

type swapT =
    {
        ethInUsdAverage: BigDecimal
        ethInUsdSum: BigDecimal
        ethOutUsdSum: BigDecimal
        pairAddress: string
        priceETH_USD: float
        priceTokenInETH: double
    }
    
let empty_swapT =
    {|
        ethInUsd = BigDecimal.Parse("0")
        pairAddress = ""
    |}
type triggerResults = { 
    pairAddress: string
    priceDifference: BigDecimal 
    volumeInUsd: BigDecimal
    ethInUsdSum: BigDecimal
    ethOutUsdSum: BigDecimal
    nameLong: string
    nameShort: string
    totalSupply: string
    priceETH_USD: float
    priceTokenInETH: float
    } with
    member this.priceDifferenceStr = this.priceDifference.RoundAwayFromZero(0) |> string
    member this.volumeInUsdStr = this.volumeInUsd.RoundAwayFromZero(0) |> string |> addDots 
    member this.ethInUsdSumStr = this.ethInUsdSum.RoundAwayFromZero(0) |> string |> addDots 
    member this.ethOutUsdSumStr = this.ethOutUsdSum.RoundAwayFromZero(0) |> string |> addDots 
    member this.ethOutInUsdSumStr = (this.ethOutUsdSum + this.ethInUsdSum).RoundAwayFromZero(0) |> string |> addDots 

    member this.totalSupplyStr = this.totalSupply|> addDots 
    member this.mkStr = let ts = if String.IsNullOrEmpty(this.totalSupply) 
                                 then "0" 
                                 else this.totalSupply
                        (BigDecimal.Parse(ts) * 
                        BigDecimal.Parse(this.priceTokenInETH.ToString("F17")) * 
                        BigDecimal.Parse(this.priceETH_USD.ToString()) )
                         .RoundAwayFromZero(0)
                        |> string 
                        |> addDots
    