module debugSettingsOption

type WsSwaps() =
    member val swapsETH = 0 with get, set
    member val swapsTokens = 0 with get, set
    member val lastBlock = 0 with get, set
    member val tokenInfo = 0 with get, set

type WsTrigger() =
    member val trigger_5mins = 0 with get, set
    member val trigger_0volumeNperiods = 0 with get, set
    member val trigger_5mins5percOfMK = 0 with get, set

type debugSettingsOption() =
    static member val SectionName = "DebugSettings" with get

    member val wsSwaps = WsSwaps() with get, set
    member val wsTrigger = WsTrigger() with get, set
    member val delayOnOff = 0 with get, set


