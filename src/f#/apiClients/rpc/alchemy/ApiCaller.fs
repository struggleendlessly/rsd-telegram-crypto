namespace alchemy

open System.Net.Http
open Microsoft.Extensions.Options
open AlchemyOptionModule
open responseGetBlockDTO
open Microsoft.Extensions.Logging
open UlrBuilder
open System
open System.Text
open System.Threading.Tasks
open System.Text.Json
open Async
open requestSingleDTO

type alchemy(
    logger: ILogger<alchemy>, 
    alchemyOptions: IOptions<AlchemyOption>, 
    httpClientFactory: IHttpClientFactory
    ) as this =

    let alchemySettings = alchemyOptions.Value;

    do Random().Shuffle alchemySettings.ApiKeys

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

            //logger.LogInformation( "response: {s}", content)

            return content
         } 
         |> Async.AwaitTask

    member this.makeRequest<'T>(index): requestSingleDTO[] ->  Async<'T> = 
        JsonSerializer.Serialize 
        >> request index 
        >> Async.map JsonSerializer.Deserialize<'T>

    member this.prepareChunks<'T>() : int[] -> 'T[]  = 
          Array.Parallel.map getBlockByNumber
          >> Array.chunkBySize 5 
          >> Array.Parallel.mapi this.makeRequest<'T>
          >> Async.Parallel
          >> Async.RunSynchronously  

    member this.getBlockByNumber  = 
        this.prepareChunks<responseGetBlocksDTO>
        

