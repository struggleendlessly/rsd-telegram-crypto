namespace CryptChat.F_

#nowarn "20"

open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http
open Telegram.Bot.Types
open CryptChat.Services
open CryptChat.Base
open System.Threading

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =
        let builder = WebApplication.CreateBuilder(args)

        builder
            .Services
            .AddControllersWithViews()
            .AddRazorRuntimeCompilation()

        builder.Services.AddRazorPages()

        let builder = WebApplication.CreateBuilder(args)
        let ngrokUrl = NgrokService.getNgrokUrlAsync()
        let webhookUrl = $"https://api.telegram.org/bot{TelegramBotService.botToken}/setWebhook?url={ngrokUrl}/webhook"
        do TelegramBotService.setWebhookAsync webhookUrl

        let app = builder.Build()

        if not (builder.Environment.IsDevelopment()) then
            app.UseExceptionHandler("/Home/Error")
            app.UseHsts() |> ignore 

        app.UseHttpsRedirection()

        app.MapPost("/webhook", fun (context: HttpContext) ->
            task {
                let! update = context.Request.ReadFromJsonAsync<Update>()

                match update.Message with
                | message ->
                    let logic = new Logic()
                    do! TelegramBotService.handleMessageAsync message logic CancellationToken.None
            } :> Task) |> ignore

        app.UseStaticFiles()
        app.UseRouting()

        app.Run()

        exitCode
