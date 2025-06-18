open System.Text
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Tweetinvi
open Tweetinvi.Models
open Newtonsoft.Json
open System.Net.Http
open System
open System.Threading.Tasks
open XTwitter.F_.Models

let buildTwitterRequest (request: PostTweetRequestDto) (client: TwitterClient) =
    System.Action<ITwitterRequest>(fun twitterRequest ->
        let jsonBody = client.Json.Serialize<PostTweetRequestDto>(request)

        let content = new StringContent(jsonBody, Encoding.UTF8, "application/json")

        twitterRequest.Query.Url <- "https://api.twitter.com/2/tweets"
        twitterRequest.Query.HttpMethod <- Tweetinvi.Models.HttpMethod.POST
        twitterRequest.Query.HttpContent <- content
    )

let postTweetHandler (ctx: HttpContext) : Task =
    task {
        use reader = new System.IO.StreamReader(ctx.Request.Body)
        let! body = reader.ReadToEndAsync()
        let request = JsonConvert.DeserializeObject<PostTweetRequestDto>(body)

        // API credentials
        let apiKey = "0PDDB3qBARvwVR6BYFVEvt11J"
        let apiSecret = "HmZRNphUWqOh0Ow2nT3VfUMLkNodotktFL0GStnQbVu2QSIYZi"
        let accessToken = "1504818697776648194-w1IbSVC47oRvV6POU2IDmLVmI3NELD"
        let accessTokenSecret = "vDIwQLojGd9Dx0ta6MGZSJI8O5XqB4vKctfWFbsmyN9L2"

        let client = new TwitterClient(apiKey, apiSecret, accessToken, accessTokenSecret)
        let! result = client.Execute.AdvanceRequestAsync(buildTwitterRequest request client)

        return! ctx.Response.WriteAsync(result.Content)
    }

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    builder.Services.AddEndpointsApiExplorer() |> ignore

    let app = builder.Build()

    app.MapPost("/api/tweets", Func<HttpContext, Task>(postTweetHandler)) |> ignore

    app.Run()
    0