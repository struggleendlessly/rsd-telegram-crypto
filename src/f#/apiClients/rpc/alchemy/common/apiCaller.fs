module apiCaller

open System.Net.Http
open System.Text
open System

let getApiKey (apiKeys: string[]) index = 
    $"/v2/{apiKeys.[index % apiKeys.Length]}"

let request 
    (apiKeys: string[])
    url
    (httpClientFactory: IHttpClientFactory)
    index 
    json : Async<string> = 

        task {
            apiKeys 
                |> Random().Shuffle

            use client = httpClientFactory.CreateClient "Api"
            client.BaseAddress <- url() |> Uri

            let httpContent = new StringContent( json, Encoding.UTF8, "application/json")

            //logger.LogInformation( "Request: {s}", json)

            let! response = client.PostAsync (getApiKey apiKeys index, httpContent )
            let! content = response.Content.ReadAsStringAsync() 

            //logger.LogInformation( "response: {s}", content)

            return content
        } 
        |> Async.AwaitTask