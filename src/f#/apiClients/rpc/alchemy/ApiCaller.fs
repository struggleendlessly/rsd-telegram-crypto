namespace alchemy

open System.Net.Http
open Microsoft.Extensions.Options
open AlchemyOptionModule
open Microsoft.Extensions.Logging
open UlrBuilder
open System

type alchemy(
    logger: ILogger<alchemy>, 
    alchemyOptions: IOptions<AlchemyOption>, 
    httpClientFactory: IHttpClientFactory
    ) =

    let alchemySettings = alchemyOptions.Value;

    let request = 
         async {
            
            let url = alchemySettings.UrlBase.Replace("{{{chainName}}}", alchemySettings.ChainNames.Etherium);

            use client = httpClientFactory.CreateClient("Api")
            client.BaseAddress <- Uri(url)
            Random().Shuffle(alchemySettings.ApiKeys)

            let! response = client.GetAsync "https://3908dca0d72445af90b8a3060008df171.api.mockbin.io" |> Async.AwaitTask 
            ""
         }

    member this.getBlockByNumber() =
        getBlockByNumber
