using Azure.Core;

using Polly;
using Polly.Extensions.Http;

using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Shared
{
    public static class PolicyHandlers
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound || msg.StatusCode == HttpStatusCode.TooManyRequests)
                .OrResult(msg => {
                    // we need it for alchem api, because sometimes they send 429 status code in the body
                    var json = msg.Content.ReadAsStringAsync().Result.Contains(":429,");

                    if (json)
                    {
                        return true;
                    }

                    return false;
                })
                .WaitAndRetryAsync(
                    6,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
