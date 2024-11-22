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

    let request json index= 
         task {
            
            let url = alchemySettings.UrlBase.Replace("{{{chainName}}}", alchemySettings.ChainNames.Etherium);

            use client = httpClientFactory.CreateClient "Api"
            client.BaseAddress <- Uri url
            let vAndKey = $"/v2/{alchemySettings.ApiKeys[0]}"

            let httpContent = new StringContent( json,Encoding.UTF8,"application/json")
            logger.LogInformation( "Request: {s}", json)
            let! response = client.PostAsync(vAndKey, httpContent)  
            let! content = response.Content.ReadAsStringAsync() 

            return content
         }
    let convertJson (json: Async<string>) : Async<'T> = 
        async {
            let! res = json
            let res = System.Text.Json.JsonSerializer.Deserialize<'T>(res)
            return res
        }
    let prepareChunks numbers =        
         numbers  
         |> Seq.chunkBySize 5 
         |> Seq.mapi (fun index value -> index, (value |> Seq.map getBlockByNumber) |> Seq.toArray |> JsonSerializer.Serialize) 
         //|> Seq.iter (fun (index, value) -> printfn "Index: %d, Value:%s" index  value  )

         |> Seq.map (fun (index, value) ->( request value index |> Async.AwaitTask ))
         |> Async.Parallel
         |> Async.RunSynchronously

    member this.getBlockByNumber() =
        prepareChunks
        // getBlockByNumber >> request >> convertJson<responseGetBlockDTO>

