using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using Shared.DB;

namespace Shared.HealthCheck
{
    public class HealthCheck
    {
        private readonly DBContext dBContext;
        private readonly IMemoryCache memoryCache;
        private readonly Telegram.Telegram telegram;
        private readonly string caheKey = "HealthCheck_15";
        public HealthCheck(
            IMemoryCache memoryCache,
            Telegram.Telegram telegram,
            DBContext dBContext)
        {
            this.memoryCache = memoryCache;
            this.telegram = telegram;
            this.dBContext = dBContext;
            telegram.SetGroup(1);
        }

        public async Task Start(string name)
        {
            var isMessageSentToBot = GetCache();
            var sholdSendMessage = DateTime.UtcNow.Minute % 10 == 0;

            if (!isMessageSentToBot && sholdSendMessage)
            {
                var today = DateTime.UtcNow.Date;
                var dbTotalCount = await dBContext.TokenInfos.Where(x => x.TimeAdded > today).CountAsync();
                var dbIsValidCount = await dBContext.TokenInfos.Where(x => x.IsValid == true && x.TimeAdded > today).CountAsync();

                var text =
                    name +
                    $"DB items today: `{dbTotalCount}` " +
                    //$"{Environment.NewLine} {Environment.NewLine}" +
                    $"DB isValid today: `{dbIsValidCount}` " +
                    $"";

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
