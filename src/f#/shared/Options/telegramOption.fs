module telegramOption

type telegramOption() =
    static member val SectionName = "Telegram" with get

    member val UrlBase = "" with get, set
    member val bot_hash: string[] = [||] with get, set
    member val api_delay_forech = 0 with get, set
    member val chat_id_coins = 0L with get, set

    member val message_thread_id_5mins = 0 with get, set
    member val message_thread_id_5mins_less100k = 0 with get, set
    member val message_thread_id_5mins_more100kLess1m = 0 with get, set
    member val message_thread_id_5mins_more1m = 0 with get, set

    member val etherscanUrl = "" with get, set
    member val dextoolsUrl = "" with get, set
    member val dexscreenerUrl = "" with get, set
    member val chainName = "" with get, set
