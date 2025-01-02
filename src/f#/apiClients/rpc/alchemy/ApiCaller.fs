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
open responseEthCall
open responseGetTransactionReceipt

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
            this.ShuffleApiKeys()
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
        this.chunksRequest<responseSwap[], int> (getSwapLogsUri_DAI ethStrings.addressDai ethStrings.topicSwap ethStrings.ethChainBlocksIn5Minutes)

    member this.getBlockSwapsETH_Tokens  = 
        this.chunksRequest<responseSwap[], int> (getSwapLogsUri_Token ethStrings.topicSwap ethStrings.ethChainBlocksIn5Minutes)

    member this.getEthCall_decimals  = 
        this.chunksRequest<responseEthCall[], string> getEthCall_decimals

    member this.getEthCall_token0  = 
        this.chunksRequest<responseEthCall[], string> getEthCall_token0

    member this.getEthCall_token1  = 
        this.chunksRequest<responseEthCall[], string> getEthCall_token1

    member this.eth_getTransactionReceipt  = 
        this.chunksRequest<responseGetTransactionReceipt[], string*string> eth_getTransactionReceipt