module solanaStrings

open System

let comparer (x: string) (y: string) = StringComparer.OrdinalIgnoreCase.Compare(x, y)
let ethChainBlocksIn5Minutes = 30
let addressDai = "0xa478c2975ab1ea89e8196811f51a7b7ade33eb11".ToLowerInvariant()
let addressETH = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2".ToLowerInvariant()
let topicSwap = "0xd78ad95fa46c994b6551d0da85fc275fe613ce37657fb8d5e3d130840159d822".ToLowerInvariant()

let ethCall_decimals = "0x313ce567"
let ethCall_token0 = "0x0dfe1681"
let ethCall_token1 = "0xd21220a7"