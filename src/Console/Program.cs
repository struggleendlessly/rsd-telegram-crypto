// See https://aka.ms/new-console-template for more information
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using System.Text.Json;
using Shared.Telegram.Models;
using static System.Net.WebRequestMethods;
//var text ="{\r\n  \"?isRandom\": \"true\",\r\n  \"count\": \"3\"\r\n}"
//    "TEST MESSAGE!!!123 \n" +

//    "`0xbf769155ff776a717fb96616a567bb898b21bee6` \n" +
//    "❤️ \n" +
//    "DB: `11178` | `14163880` | `14190231` | 26351 \n" +

//    "[Owner](https://basescan.org/address/0xc0feb38ca691a7ccd508832571dc26b51de500e3) | [Token](https://basescan.org/token/0xbf769155ff776a717fb96616a567bb898b21bee6) | [DexScreener](https://dexscreener.com/base/0xbf769155ff776a717fb96616a567bb898b21bee6)";

//var ttt = "https://api.telegram.org/bot6721227973:AAHGbb1gjBn9CWh0zF9sOtVKA0g6iPp9KCE/sendMessage?message_thread_id=2268&chat_id=-1002144699173&text=1111&parse_mode=MarkDown&disable_web_page_preview=true";
//string urlString = $"https://api.telegram.org/bot6721227973:AAHGbb1gjBn9CWh0zF9sOtVKA0g6iPp9KCE/sendMessage?message_thread_id=136&chat_id=-1002144699173&text={text}&parse_mode=markdown&disable_web_page_preview=true";
//string deleteMessage = "https://api.telegram.org/bot6721227973:AAHGbb1gjBn9CWh0zF9sOtVKA0g6iPp9KCE/deleteMessage?message_thread_id=136&chat_id=-1002144699173&message_id=2802";
//using (var webclient = new WebClient())
//{
//    try
//    {
//        var response = await webclient.DownloadStringTaskAsync(urlString);
//        var ee = JsonSerializer.Deserialize<MessageSend>(response);
//    }
//    catch (Exception ee)
//    {

//        throw;
//    }

//}

////https://api.telegram.org/bot6721227973:AAHGbb1gjBn9CWh0zF9sOtVKA0g6iPp9KCE/deleteMessage?message_thread_id=136&chat_id=-1002144699173&message_id=2800
Console.WriteLine("Hello, World!");



