namespace CryptChat.Services

module TelegramBotService =
    open System
    open System.Net.Http
    open System.Threading
    open System.Threading.Tasks
    open Telegram.Bot
    open Telegram.Bot.Types
    open CryptChat.Base

    let botToken = "8130511485:AAHeCpnorWzj3ZYJPsDo-TE_bGncdqiJlTk"
    let botClient = TelegramBotClient(botToken)

    let setWebhookAsync (webhookUrl: string) =
        task {
            use client = new HttpClient()
            let! _ = client.PostAsync(webhookUrl, null)
            Console.WriteLine("Webhook set successfully!")
        }

    let handleMessageAsync (message: Message) (logic: Logic) (cancellationToken: CancellationToken) : Task =
        task {
            let chatId = message.Chat.Id
            let userId = message.From.Id
            let mutable isAdmin = false

            match message.Text with
            | text when text.StartsWith("/") -> 
                let! admins = botClient.GetChatAdministrators(ChatId.op_Implicit chatId)
                isAdmin <- admins |> Seq.exists (fun admin -> admin.User.Id = userId)
            | _ -> ()

            match message.Text with
            | text when text.StartsWith("/start") -> do! logic.StartAsync(message, botClient)
            | text when text.StartsWith("/lastBlock") -> 
                let responseMessage = if isAdmin then "Вы админ" else "Вы не админ"
                do! botClient.SendMessage(ChatId.op_Implicit chatId, responseMessage, cancellationToken = cancellationToken) :> Task
            | _ -> return ()
        }

