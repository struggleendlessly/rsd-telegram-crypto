﻿namespace alchemy

open System.Net.Http
open System.Text.Json

open Microsoft.Extensions.Options
open Microsoft.Extensions.Logging

open UlrBuilderEVM
open AlchemyOptionModule
open responseGetBlock
open requestSingleDTO
open responseGetLastBlock
open responseSwap
open responseEthCall
open responseGetTransactionReceipt
open ChainSettingsOptionModule
open apiCaller
open responseTokenMetadata

type alchemyEVM(
    logger: ILogger<alchemyEVM>, 
    alchemyOptions: IOptions<AlchemyOption>, 
    chainSettingsOption: IOptions<ChainSettingsOption>, 
    httpClientFactory: IHttpClientFactory
    ) as this =

    let alchemySettings = alchemyOptions.Value;
    let chainSettingsOption = chainSettingsOption.Value;

    let url() = alchemySettings.UrlBase.Replace("{{{chainName}}}", this.chainName);

    member val chainName = "" with get, set

    member private this.chunksRequest<'T, 'B> uriBuilder : 'B seq -> Async<'T[]>  = 
        Seq.map uriBuilder 
        >> Seq.chunkBySize 50 
        >> Seq.mapi this.makeRequest<'T>
        >> Async.Parallel

    member private this.makeRequest<'T>(index): requestSingleDTO [] -> Async<'T> = 
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

    member this.getLastBlockNumber  = 
        this.singleRequest<responseGetLastBlock> getLastBlockNumberUri

    member this.getBlockByNumber  = 
        this.chunksRequest<responseGetBlock seq, int> getBlockByNumberUri

    member this.getBlockSwapsETH_USD = 
        this.chunksRequest<responseSwap seq, int> (getSwapLogsUri_DAI chainSettingsOption.AddressStableCoin chainSettingsOption.TopicSwap chainSettingsOption.BlocksIn5Minutes)

    member this.getBlockSwapsETH_Tokens = 
        this.chunksRequest<responseSwap seq, int> (getSwapLogsUri_Token chainSettingsOption.TopicSwap chainSettingsOption.BlocksIn5Minutes)

    member this.getEthCall_decimals  = 
        this.chunksRequest<responseEthCall seq, string> (getEthCall_decimals chainSettingsOption.EthCall_decimals)

    member this.getTotalSupply  = 
        this.chunksRequest<responseEthCall seq, string> (getTotalSupply chainSettingsOption.EthCall_totalSupply)

    member this.getTokenNames  = 
        this.chunksRequest<responseTokenMetadata seq, string> getTokenNames 

    member this.getEthCall_token0  = 
        this.chunksRequest<responseEthCall seq, string> (getEthCall_token0 chainSettingsOption.EthCall_token0)

    member this.getEthCall_token1  = 
        this.chunksRequest<responseEthCall seq, string> (getEthCall_token1 chainSettingsOption.EthCall_token1)

    member this.eth_getTransactionReceipt  = 
        this.chunksRequest<responseGetTransactionReceipt seq, string*string> eth_getTransactionReceipt