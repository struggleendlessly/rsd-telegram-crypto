module baseStrings

open System

let baseChainBlocksIn5Minutes = 150
let addressDai = "0x88a43bbdf9d098eec7bceda4e2494615dfd9bb9c".ToLowerInvariant() // usdc/weth
let addressETH = "0x4200000000000000000000000000000000000006".ToLowerInvariant()
let topicSwap = "0xd78ad95fa46c994b6551d0da85fc275fe613ce37657fb8d5e3d130840159d822".ToLowerInvariant()
let defaultBlockNumber = 24567082

let ethCall_decimals = "0x313ce567"
let ethCall_token0 = "0x0dfe1681"
let ethCall_token1 = "0xd21220a7"