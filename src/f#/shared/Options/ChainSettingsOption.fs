module ChainSettingsOptionModule

type ChainSettingsOption() =
    let mutable internalAddressStableCoin = ""
    let mutable internalAddressChainCoin = ""
    let mutable internalTopicSwap = ""
    let mutable internalEthCallDecimals = ""
    let mutable internalEthCallTotalSupply = ""
    let mutable internalEthCallToken0 = ""
    let mutable internalEthCallToken1 = ""
    let mutable internalExcludedAddresses = [||]
    let mutable internalAddressStableCoinsToInteract = [||]
    
    member this.AddressStableCoin
        with get() = internalAddressStableCoin
        and set(value:string) = internalAddressStableCoin <- value.ToLowerInvariant()

    member this.AddressChainCoin
        with get() = internalAddressChainCoin
        and set(value:string) = internalAddressChainCoin <- value.ToLowerInvariant()

    member this.TopicSwap
        with get() = internalTopicSwap
        and set(value:string) = internalTopicSwap <- value.ToLowerInvariant()

    member this.EthCall_decimals
        with get() = internalEthCallDecimals
        and set(value:string) = internalEthCallDecimals <- value.ToLowerInvariant()

    member this.EthCall_totalSupply
        with get() = internalEthCallTotalSupply
        and set(value:string) = internalEthCallTotalSupply <- value.ToLowerInvariant()

    member this.EthCall_token0
        with get() = internalEthCallToken0
        and set(value:string) = internalEthCallToken0 <- value.ToLowerInvariant()

    member this.EthCall_token1
        with get() = internalEthCallToken1
        and set(value:string) = internalEthCallToken1 <- value.ToLowerInvariant()

    member this.ExcludedAddresses
        with get() = internalExcludedAddresses
        and set(value:string []) = internalExcludedAddresses <- value |> Array.map (fun x -> x.ToLowerInvariant())    
    
    member this.AddressStableCoinsToInteract
        with get() = internalAddressStableCoinsToInteract
        and set(value:string []) = internalAddressStableCoinsToInteract <- value |> Array.map (fun x -> x.ToLowerInvariant())

    member val DefaultBlockNumber: uint64 = 0UL with get, set
    member val BlocksIn5Minutes = 0 with get, set
    member val AddressChainCoinDecimals = 0 with get, set
    static member val SectionName = "ChainSettings" with get

