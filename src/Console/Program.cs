// See https://aka.ms/new-console-template for more information
using TL;

using var client = new WTelegram.Client(Config);
var myself = await client.LoginUserIfNeeded();
//Console.WriteLine($"We are logged-in as {myself} (id {myself.id})");
var chats = await client.Messages_GetAllChats();
var chatBaseNewTokenId = "1958915778";
InputPeer peerBaseNewToken = chats.chats[1958915778];

for (int offset_id = 0; ;)
{
    var messages = await client.Messages_GetHistory(peerBaseNewToken, offset_id);
    if (messages.Messages.Length == 0) break;
    foreach (var msgBase in messages.Messages)
    {
        var from = messages.UserOrChat(msgBase.From ?? msgBase.Peer); // from can be User/Chat/Channel
        if (msgBase is Message msg)
        {
            //Console.WriteLine($"{from}> {msg.message} {msg.media}");
            //Console.WriteLine(Environment.NewLine);
        }
        else if (msgBase is MessageService ms)
        {

        }
            //Console.WriteLine($"{from} [{ms.action.GetType().Name[13..]}]");
    }
    offset_id = messages.Messages[^1].ID;
}

//Console.WriteLine("Hello, World!");
static string Config(string what)
{
    switch (what)
    {
        case "api_id": return "23880234";
        case "api_hash": return "c349bbf6c93c7df984219a77aeb320df";
        case "phone_number": return "+380996000291";
        case "server_address": return "2>149.154.167.50:443";
        //case "verification_code": Console.Write("Code: "); return Console.ReadLine();
        case "first_name": return "John";      // if sign-up is required
        case "last_name": return "Doe";        // if sign-up is required
        case "password": return "secret!";     // if user has enabled 2FA
        default: return null;                  // let WTelegramClient decide the default config
    }

    //switch (what)
    //{
    //    case "api_id": return "9969324";
    //    case "api_hash": return "6822f6af521f6089a20307cf1e9e7e3b";
    //    case "phone_number": return "+34672090908";
    //    case "server_address": return "2>149.154.167.50:443";
    //    case "verification_code": Console.Write("Code: "); return Console.ReadLine();
    //    case "first_name": return "John";      // if sign-up is required
    //    case "last_name": return "Doe";        // if sign-up is required
    //    case "password": return "secret!";     // if user has enabled 2FA
    //    default: return null;                  // let WTelegramClient decide the default config
    //}
}