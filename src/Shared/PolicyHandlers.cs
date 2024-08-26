using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

using System.Net;

namespace Shared
{
    public static class PolicyHandlers
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
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
