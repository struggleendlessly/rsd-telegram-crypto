using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.TelegramApi
{
    public class MessagesRead
    {
        public MessagesRead()
        {

        }

        static string Config(string what)
        {
            switch (what)
            {
                case "api_id": return "23880234";
                case "api_hash": return "c349bbf6c93c7df984219a77aeb320df";
                case "phone_number": return "+380996000291";
                case "server_address": return "2>149.154.167.40:443";
                case "verification_code": Console.Write("Code: "); return Console.ReadLine();
                //case "first_name": return "John";      // if sign-up is required
                //case "last_name": return "Doe";        // if sign-up is required
                //case "password": return "secret!";     // if user has enabled 2FA
                default: return null;                  // let WTelegramClient decide the default config
            }
        }

        public async Task Start()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.c-sharpcorner.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                //GET Method
                HttpResponseMessage response = await client.GetAsync("article/calling-web-api-using-httpclient/");
                if (response.IsSuccessStatusCode)
                {
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
            }

            Console.WriteLine("MessagesRead started");

            //using var client = new WTelegram.Client(Config);
            
            //var myself = await client.LoginUserIfNeeded();
            //Console.WriteLine($"Logged in as {myself.last_name}");
            // Create a new instance of the Telegram client
            //using (var client = new WTelegram.Client())
            //{
            //    // Connect to the Telegram server
            //    await client.ConnectAsync();

            //    // Get the public chat ID
            //    var chatId = await client.me.GetMessages.GetPublicChatIdAsync("public_chat_username");

            //    // Get the latest messages from the public chat
            //    var messages = await client.GetChatMessagesAsync(chatId);

            //    // Process the messages
            //    foreach (var message in messages)
            //    {
            //        Console.WriteLine(message.Text);
            //    }
            //}

            //const int TargetChatId = 1175259547; // Replace with your chat ID
            //using var client = new WTelegram.Client();

            //client.UpdateMessage += (sender, updates) =>
            //{
            //    switch (updates.constructor)
            //    {
            //        case Constructor.UpdatesTooLong:
            //            // Handle updates
            //            break;
            //        case Constructor.UpdateShortMessage:
            //            // Handle short messages
            //            break;
            //        case Constructor.UpdateShortChatMessage:
            //            // Handle short chat messages
            //            break;
            //            // Add other cases for different update types as needed
            //    }
            //};

            //await client.Start();
        }
    }
}
