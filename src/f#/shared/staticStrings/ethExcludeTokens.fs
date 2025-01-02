module ethExcludeTokens

let tokensExclude =
    [| 
         "0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2".ToLowerInvariant() //WETH
         "0xb4e16d0168e52d35cacd2c6185b44281ec28c9dc".ToLowerInvariant() //WETH/USDC v2
         "0x0d4a11d5eeaac28ec3f61d100daf4d40471f1852".ToLowerInvariant() //WETH/USDT v2
         "0xc3d03e4f041fd4cd388c549ee2a29a9e5075882f".ToLowerInvariant() //WETH/DAI v2
         "0xa0b86991c6218b36c1d19d4a2e9eb0ce3606eb48".ToLowerInvariant() //USDC/DAI 
         "0x2e8135be71230c6b1b4045696d41c09db0414226".ToLowerInvariant() //WETH/USDC
         "0x9c2dc3d5ffcecf61312c5f4c00660695b32fb3d1".ToLowerInvariant() //WETH/USDC
         "0x3aa370aacf4cb08c7e1e7aa8e8ff9418d73c7e0f".ToLowerInvariant() //WETH/USDC
         "0x397ff1542f962076d0bfe58ea045ffa2d347aca0".ToLowerInvariant() //WETH/USDC
         "0xdac17f958d2ee523a2206206994597c13d831ec7".ToLowerInvariant() //USDT/USDC
         "0x2260fac5e5542a773aa44fbcfedf7c193bc2c599".ToLowerInvariant() //WBTC/USDT
         "0xa029a744b4e44e22f68a1bb9a848caafbf6bb233".ToLowerInvariant() //WETH/USDT
         "0x008d9d457ad0a75c83d9a5e2b8e4ce89232b5083".ToLowerInvariant() //WETH/USDT
         "0x048f0e7ea2cfd522a4a058d1b1bdd574a0486c46".ToLowerInvariant() //WETH/USDT
    |]