// See https://aka.ms/new-console-template for more information
using static System.Net.Mime.MediaTypeNames;
using System.Net;

var text = """
    TEST MESSAGE!!!

    `0xbf769155ff776a717fb96616a567bb898b21bee6`

    DB: `11178` | `14163880` | `14190231` | 26351 

    [Owner](https://basescan.org/address/0xc0feb38ca691a7ccd508832571dc26b51de500e3) | [Token](https://basescan.org/token/0xbf769155ff776a717fb96616a567bb898b21bee6) | [DexScreener](https://dexscreener.com/base/0xbf769155ff776a717fb96616a567bb898b21bee6)
    
    """;

string urlString = $"https://api.telegram.org/bot6721227973:AAHGbb1gjBn9CWh0zF9sOtVKA0g6iPp9KCE/sendMessage?message_thread_id=136&chat_id=-1002144699173&text={text}&parse_mode=markdown";

using (var webclient = new WebClient())
{
    var response = await webclient.DownloadStringTaskAsync(urlString);

}

Console.WriteLine("Hello, World!");

