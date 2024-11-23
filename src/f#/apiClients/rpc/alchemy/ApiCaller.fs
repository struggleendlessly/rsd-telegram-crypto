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




type alchemy(
    logger: ILogger<alchemy>, 
    alchemyOptions: IOptions<AlchemyOption>, 
    httpClientFactory: IHttpClientFactory
    ) as this =

    let alchemySettings = alchemyOptions.Value;

    do Random().Shuffle alchemySettings.ApiKeys

    let getApiKey index = 
        $"/v2/{alchemySettings.ApiKeys.[index % alchemySettings.ApiKeys.Length]}"

    let request index json= 
         task {
            
            let url = alchemySettings.UrlBase.Replace("{{{chainName}}}", alchemySettings.ChainNames.Etherium);

            use client = httpClientFactory.CreateClient "Api"
            client.BaseAddress <- Uri url

            let httpContent = new StringContent( json, Encoding.UTF8, "application/json")

            logger.LogInformation( "Request: {s}", json)

            let! response = client.PostAsync(getApiKey index, httpContent)  
            let! content = response.Content.ReadAsStringAsync() 

            //logger.LogInformation( "response: {s}", content)

            return content
         }

    let convertJson (json: Async<string>) : Async<'T> = 
        async {
            let! res = json
            let res = System.Text.Json.JsonSerializer.Deserialize<'T>(res)
            return res
        }

    let prepareChunks blocks =        
         blocks 
         |> Array.Parallel.map getBlockByNumber
         |> Array.chunkBySize 5 
         |> Array.Parallel.mapi (fun index value -> value |> JsonSerializer.Serialize |> request index |> Async.AwaitTask) 
         |> Async.Parallel
         |> Async.RunSynchronously

    member this.getBlockByNumber() =
        prepareChunks
        // getBlockByNumber >> request >> convertJson<responseGetBlockDTO>

