module bl_others

open Nethereum.Util
open System.Text.RegularExpressions

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
        ethInUsd: BigDecimal
        pairAddress: string
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
    } with
    member this.priceDifferenceStr = this.priceDifference.RoundAwayFromZero(0) |> string
    member this.volumeInUsdStr = this.volumeInUsd.RoundAwayFromZero(0) |> string |> addDots 
    