module AppSettingsOption

type LogLevel() =
    member val Default = "" with get, set

type Logging() =
    member val LogLevel = LogLevel() with get, set

type AppSettings() =
    member val Logging = Logging() with get, set