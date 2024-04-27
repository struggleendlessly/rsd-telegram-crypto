using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using WTelegram;

namespace Console
{
    internal class basescan
    {
        string baseUrl = "https://api.basescan.org/api";
        string apiKeyToken = "VPD99DPE6Z57DP1QNYRXAUS4PHR54B1QZW";
        public basescan()
        {

        }

        public async string GetInfoByOwnerAddress(string address)
        {
            string url =
                 $"""
                    /?module=account 
                     &action=txlist
                     &address={address}
                     &startblock=0
                     &endblock=99999999
                     &page=1
                     &offset=100
                     &sort=asc
                     &apikey={apiKeyToken}
                 """;

            HttpClient sharedClient = new()
            {
                BaseAddress = new Uri(baseUrl),
            };

            using HttpResponseMessage response = await sharedClient.GetAsync(url);
            response.EnsureSuccessStatusCode()
                 .WriteRequestToConsole();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{jsonResponse}\n")


        }

    }
    static class HttpResponseMessageExtensions
    {
        internal static void WriteRequestToConsole(this HttpResponseMessage response)
        {
            if (response is null)
            {
                return;
            }

            var request = response.RequestMessage;
            Console.Write($"{request?.Method} ");
            Console.Write($"{request?.RequestUri} ");
            Console.WriteLine($"HTTP/{request?.Version}");
        }
    }
}
