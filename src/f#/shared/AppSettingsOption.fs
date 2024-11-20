module AppSettingsOptionModule

type LogLevel() =
    member val Default = "" with get, set

type Logging() =
    member val LogLevel = LogLevel() with get, set

type AppSettingsOption() =
    static member val SectionName = "Logging" with get
    member val Logging = Logging() with get, set