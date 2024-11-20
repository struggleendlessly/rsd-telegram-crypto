module OpenTelemetryOptionModule

type OpenTelemetryOption() =
    static member val SectionName = "OpenTelemetry" with get

    member val Url =  "" with get, set
    member val ApiKey =  "" with get, set
    member val ServiceName =  "" with get, set
