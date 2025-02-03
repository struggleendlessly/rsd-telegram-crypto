module configurationExtensions

open System
open System.Net
open System.Net.Http
open System.Collections.Generic

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection

open Polly
open Polly.Timeout
open Polly.Extensions.Http

open AppSettingsOptionModule
open telemetryOption
open AlchemyOptionModule
open ChainSettingsOptionModule
open telegramOption

open IScopedProcessingService
open scoped_trigger_0volumeNperiods
open scoped_trigger_5mins5percOfMK
open scopedTokenInfo
open scopedSwapsETH
open scopedLastBlock
open scopedSwapsTokens
open scopedNames
open scoped_trigger_5mins
open scoped_telegram
open debugSettingsOption

let getRetryPolicy() : IAsyncPolicy<HttpResponseMessage> =
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .Or<TimeoutRejectedException>()
        .OrResult(fun msg -> msg.StatusCode = HttpStatusCode.NotFound || msg.StatusCode = HttpStatusCode.TooManyRequests)
        .OrResult(fun msg -> 
            let json = msg.Content.ReadAsStringAsync().Result.Contains(":429,")
            //let json1 = msg.Content.ReadAsStringAsync().Result.Contains("-32600")
            //if json1 
            //then true 
            //else false
            json
        )
        .WaitAndRetryAsync(
            6,
            fun retryAttempt -> TimeSpan.FromSeconds(Math.Pow(2.0, float retryAttempt))
        )

let uncurry2 f = fun x y -> f (x, y)
let exponentially = float >> (uncurry2 Math.Pow 2) >> TimeSpan.FromSeconds

let configureServices (services: IServiceCollection) (configuration: IConfiguration) =
    
    services.Configure<debugSettingsOption>(configuration.GetSection(debugSettingsOption.SectionName)) |> ignore
    services.Configure<AppSettingsOption>(configuration.GetSection(AppSettingsOption.SectionName)) |> ignore
    services.Configure<telegramOption>(configuration.GetSection(telegramOption.SectionName)) |> ignore
    services.Configure<telemetryOption>(configuration.GetSection(telemetryOption.SectionName)) |> ignore
    services.Configure<AlchemyOption>(configuration.GetSection(AlchemyOption.SectionName)) |> ignore
    services.Configure<ChainSettingsOption>(configuration.GetSection(ChainSettingsOption.SectionName)) |> ignore

    services.AddHttpClient("Api")  
        .ConfigurePrimaryHttpMessageHandler(
            Func<HttpMessageHandler>(
                fun _ -> new SocketsHttpHandler(MaxConnectionsPerServer = 1000)
            ))
        .SetHandlerLifetime(TimeSpan.FromMinutes 5.0)
        .AddPolicyHandler(getRetryPolicy())
        .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, fun retryAttempt -> exponentially retryAttempt)) |> ignore

    services.AddScoped<scopedTokenInfo>() |> ignore
    services.AddScoped<scopedSwapsETH>() |> ignore
    services.AddScoped<scopedSwapsTokens>() |> ignore
    services.AddScoped<scopedLastBlock>() |> ignore
    services.AddScoped<scoped_telegram>() |> ignore

    services.AddScoped<scoped_trigger_5mins>() |> ignore
    services.AddScoped<scoped_trigger_5mins5percOfMK>() |> ignore
    services.AddScoped<scoped_trigger_0volumeNperiods>() |> ignore

    services.AddScoped<IDictionary<string, IScopedProcessingService>>(
        fun sp -> 
            let dict = new Dictionary<string, IScopedProcessingService>() 
            dict.Add(scopedSwapsETHName, sp.GetRequiredService<scopedSwapsETH>() :> IScopedProcessingService)
            dict.Add(scopedSwapsTokensName, sp.GetRequiredService<scopedSwapsTokens>() :> IScopedProcessingService) 
            dict.Add(scopedLastBlockName, sp.GetRequiredService<scopedLastBlock>() :> IScopedProcessingService) 
            dict.Add(scoped_tokenInfo_Name, sp.GetRequiredService<scopedTokenInfo>() :> IScopedProcessingService) 

            dict.Add(scoped_trigger_5mins_Name, sp.GetRequiredService<scoped_trigger_5mins>() :> IScopedProcessingService) 
            dict.Add(scoped_trigger_5mins5percOfMK_Name, sp.GetRequiredService<scoped_trigger_5mins5percOfMK>() :> IScopedProcessingService) 
            dict.Add(scoped_trigger_0volumeNperiods_Name, sp.GetRequiredService<scoped_trigger_0volumeNperiods>() :> IScopedProcessingService) 

            dict :> IDictionary<string, IScopedProcessingService> ) |> ignore
