namespace alchemy

open System
open System.Net.Http
open System.Text
open System.Text.Json

open Microsoft.Extensions.Options
open Microsoft.Extensions.Logging

open UlrBuilder
open AlchemyOptionModule
open responseGetBlock
open requestSingleDTO
open responseGetLastBlock
open responseSwap
open Extensions
open responseSwap

type alchemy(
    logger: ILogger<alchemy>, 
    alchemyOptions: IOptions<AlchemyOption>, 
    httpClientFactory: IHttpClientFactory
    ) as this =

    let alchemySettings = alchemyOptions.Value;

    //do this.ShuffleApiKeys()

    let getApiKey index = 
        $"/v2/{alchemySettings.ApiKeys.[index % alchemySettings.ApiKeys.Length]}"

    let request index json : Async<string>= 
         task {
            
            let url = alchemySettings.UrlBase.Replace("{{{chainName}}}", alchemySettings.ChainNames.Etherium);

            use client = httpClientFactory.CreateClient "Api"
            client.BaseAddress <- Uri url

            let httpContent = new StringContent( json, Encoding.UTF8, "application/json")

            logger.LogInformation( "Request: {s}", json)

            let! response = client.PostAsync (getApiKey index, httpContent )
            let! content = response.Content.ReadAsStringAsync() 

            logger.LogInformation( "response: {s}", content)

            return content
         } 
         |> Async.AwaitTask

    member this.ShuffleApiKeys()  = 
         Random().Shuffle alchemySettings.ApiKeys

    member private this.chunksRequest<'T, 'B> uriBuilder : 'B[] -> Async<'T[]>  = 
        Array.map uriBuilder 
        >> Array.chunkBySize 50 
        >> Array.mapi this.makeRequest<'T>
        >> Async.Parallel

    member private this.makeRequest<'T>(index): requestSingleDTO[] -> Async<'T> = 
        JsonSerializer.Serialize 
        >> request index 
        >> Async.map JsonSerializer.Deserialize<'T>

    member private this.singleRequest<'T> uriBuilder : unit -> Async<'T> =
        uriBuilder 
        >> JsonSerializer.Serialize 
        >> request 0 
        >> Async.map JsonSerializer.Deserialize<'T>

    member this.getLastBlockNumber  = 
        this.singleRequest<responseGetLastBlock> getLastBlockNumberUri

    member this.getBlockByNumber  = 
        this.chunksRequest<responseGetBlocks, int> getBlockByNumberUri

    member this.getBlockSwapsETH_USD  = 
        let contractAddress = "0xa478c2975ab1ea89e8196811f51a7b7ade33eb11";
        let topic = "0xd78ad95fa46c994b6551d0da85fc275fe613ce37657fb8d5e3d130840159d822"
        this.chunksRequest<responseSwap[], int> (getSwapLogsUri contractAddress topic)