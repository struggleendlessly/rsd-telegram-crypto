﻿module telemetryOption

type telemetryOption() =
    static member val SectionName = "OpenTelemetry" with get

    member val Url =  "" with get, set
    member val ApiKey =  "" with get, set
    member val ServiceName =  "" with get, set

    member val UrlBase = "" with get, set
    member val bot_hash: string[] = [||] with get, set
    member val api_delay_forech = 0 with get, set
    member val chat_id_coins = 0L with get, set
    member val message_thread_id_5mins = 0 with get, set
    member val etherscanUrl = "" with get, set
    member val dextoolsUrl = "" with get, set
    member val dexscreenerUrl = "" with get, set
