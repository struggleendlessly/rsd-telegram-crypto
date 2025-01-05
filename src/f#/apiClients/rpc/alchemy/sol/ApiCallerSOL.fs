module ApiCallerSOL

open System.Net.Http
open System.Text.Json

open Microsoft.Extensions.Options
open Microsoft.Extensions.Logging

open apiCaller
open UlrBuilderSOL
open AlchemyOptionModule
open ChainSettingsOptionModule

open requestSingleDTO

open responseGetSlots

type alchemySOL(
    logger: ILogger<alchemySOL>, 
    alchemyOptions: IOptions<AlchemyOption>, 
    chainSettingsOption: IOptions<ChainSettingsOption>, 
    httpClientFactory: IHttpClientFactory
    ) as this =

    let alchemySettings = alchemyOptions.Value;
    let chainSettingsOption = chainSettingsOption.Value;

    let url() = alchemySettings.UrlBase.Replace("{{{chainName}}}", this.chainName);

    member val chainName = "" with get, set

    member private this.chunksRequest<'T, 'B> uriBuilder : 'B[] -> Async<'T[]>  = 
        Array.map uriBuilder 
        >> Array.chunkBySize 50 
        >> Array.mapi this.makeRequest<'T>
        >> Async.Parallel

    member private this.makeRequest<'T>(index): requestSingleDTO[] -> Async<'T> = 
        JsonSerializer.Serialize 
        >> request 
                logger
                alchemySettings.ApiKeys 
                url  
                httpClientFactory 
                index 
        >> Async.map JsonSerializer.Deserialize<'T>

    member private this.singleRequest<'T> uriBuilder : unit -> Async<'T> =
        uriBuilder 
        >> JsonSerializer.Serialize 
        >> request 
                 logger
                 alchemySettings.ApiKeys 
                 url
                 httpClientFactory 
                 0 
        >> Async.map JsonSerializer.Deserialize<'T>

    member this.getLastSlot  = 
        this.singleRequest<responseGetSlots> getSlotLeader
