module AlchemyOptionModule

type ChainNames() =
    member val Etherium = "" with get, set
    member val Base = "" with get, set

type AlchemyOption() =
    static member val SectionName = "ApiAlchemy" with get

    member val UrlBaseLocalNode = "" with get, set
    member val UrlBase = "" with get, set
    member val UrlVersion = "" with get, set
    member val ApiKeys : string[] = [||] with get, set
    member val ChainNames = ChainNames() with get, set
