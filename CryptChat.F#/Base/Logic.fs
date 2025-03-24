namespace CryptChat.Base

open System.Threading.Tasks
open Telegram.Bot
open Telegram.Bot.Types

    type Logic() =
        member _.StartAsync(message: Message, client: TelegramBotClient) : Task =
            task {
                let str = "C R Y P T C H A T v. 1.0\n--------------------------------------\nКоманды для работы:\n/lastBlock - начать поиск по валюте"
                do! client.SendMessage(ChatId(message.Chat.Id), str) :> Task
            }

