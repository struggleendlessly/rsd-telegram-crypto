open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Telegram.Bot.Types
open CryptChat.Services
open System.Threading.Tasks
open CryptChat.Base
open System.Threading

[<EntryPoint>]
let main args =
    task {
        let builder = WebApplication.CreateBuilder(args)
        let! ngrokUrl = NgrokService.getNgrokUrlAsync()
        let webhookUrl = $"https://api.telegram.org/bot{TelegramBotService.botToken}/setWebhook?url={ngrokUrl}/webhook"
        do! TelegramBotService.setWebhookAsync webhookUrl

        let app = builder.Build()
        app.UseHttpsRedirection()

        app.MapPost("/webhook", fun (context: HttpContext) ->
            task {
                let! update = context.Request.ReadFromJsonAsync<Update>()

                match update.Message with
                | message ->
                    let logic = new Logic()
                    do! TelegramBotService.handleMessageAsync message logic CancellationToken.None
            } :> Task) |> ignore

        app.Run()
    }
    |> Async.AwaitTask
    |> Async.RunSynchronously
    0