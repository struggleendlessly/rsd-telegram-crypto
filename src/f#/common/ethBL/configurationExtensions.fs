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
open OpenTelemetryOptionModule
open AlchemyOptionModule
open ChainSettingsOptionModule

open IScopedProcessingService
open scopedTokenInfo
open scopedSwapsETH
open scopedLastBlock
open scopedSwapsTokens
open scopedNames

let getRetryPolicy() : IAsyncPolicy<HttpResponseMessage> =
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .Or<TimeoutRejectedException>()
        .OrResult(fun msg -> msg.StatusCode = HttpStatusCode.NotFound || msg.StatusCode = HttpStatusCode.TooManyRequests)
        .OrResult(fun msg -> 
            let json = msg.Content.ReadAsStringAsync().Result.Contains(":429,")
            json
        )
        .WaitAndRetryAsync(
            6,
            fun retryAttempt -> TimeSpan.FromSeconds(Math.Pow(2.0, float retryAttempt))
        )

let uncurry2 f = fun x y -> f (x, y)
let exponentially = float >> (uncurry2 Math.Pow 2) >> TimeSpan.FromSeconds

let configureServices (services: IServiceCollection) (configuration: IConfiguration) =
    
    services.Configure<AppSettingsOption>(configuration.GetSection(AppSettingsOption.SectionName)) |> ignore
    services.Configure<OpenTelemetryOption>(configuration.GetSection(OpenTelemetryOption.SectionName)) |> ignore
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

    services.AddScoped<IDictionary<string, IScopedProcessingService>>(
        fun sp -> 
            let dict = new Dictionary<string, IScopedProcessingService>() 
            dict.Add(scopedSwapsETH, sp.GetRequiredService<scopedSwapsETH>() :> IScopedProcessingService)
            dict.Add(scopedSwapsTokens, sp.GetRequiredService<scopedSwapsTokens>() :> IScopedProcessingService) 
            dict.Add(scopedLastBlock, sp.GetRequiredService<scopedLastBlock>() :> IScopedProcessingService) 
            dict :> IDictionary<string, IScopedProcessingService> ) |> ignore
