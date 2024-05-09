using Microsoft.Extensions.Caching.Memory;

namespace Shared.HealthCheck
{
    public class HealthCheck
    {
        private readonly IMemoryCache memoryCache;
        private readonly Telegram.Telegram telegram;
        private readonly string caheKey = "HealthCheck_15";
        public HealthCheck(IMemoryCache memoryCache, Telegram.Telegram telegram)
        {
            this.memoryCache = memoryCache;
            this.telegram = telegram;
            telegram.SetGroup(1);
        }

        public async Task Start(string name)
        {
            var isMessageSentToBot = GetCache();
            var sholdSendMessage = DateTime.UtcNow.Minute % 10 == 0;

            if (!isMessageSentToBot && sholdSendMessage)
            {
                var text = "HealthCheck: " + name;
                await telegram.SendMessageToGroup(text);
                SetCache();
            }
        }

        private void SetCache()
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
            memoryCache.Set(caheKey, true, cacheEntryOptions);
        }

        private bool GetCache()
        {
            if (!memoryCache.TryGetValue(caheKey, out bool isMessageSentToBot))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
                isMessageSentToBot = false;

                memoryCache.Set(caheKey, isMessageSentToBot, cacheEntryOptions);
            }

            return isMessageSentToBot;
        }
    }
}
