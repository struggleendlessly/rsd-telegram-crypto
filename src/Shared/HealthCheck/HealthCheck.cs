using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;
using Shared.DB;

namespace Shared.HealthCheck
{
    public class HealthCheck
    {
        private readonly DBContext dBContext;
        private readonly IMemoryCache memoryCache;
        private readonly Telegram.Telegram telegram;
        private readonly BaseScan.BaseScanApiClient baseScan;
        private readonly OptionsTelegram optionsTelegram;

        private readonly string caheKey = "HealthCheck_15";
        public HealthCheck(
            IMemoryCache memoryCache,
            Telegram.Telegram telegram,
            DBContext dBContext,
            BaseScan.BaseScanApiClient baseScan,
            IOptions<OptionsTelegram> optionsTelegram)
        {
            this.memoryCache = memoryCache;
            this.telegram = telegram;
            this.dBContext = dBContext;
            this.baseScan = baseScan;
            this.optionsTelegram = optionsTelegram.Value;

            telegram.SetGroup(1);
        }

        public async Task StartWithInfo(string name)
        {
            var isMessageSentToBot = GetCache();
            var sholdSendMessage = DateTime.UtcNow.Minute % 10 == 0;

            if (!isMessageSentToBot && sholdSendMessage)
            {
                var today = DateTime.UtcNow.Date;
                var dbTotalCount = await dBContext.TokenInfos.Where(x => x.TimeAdded > today).CountAsync();
                var dbIsValidCount = await dBContext.TokenInfos.Where(x => x.IsValid == true && x.TimeAdded > today).CountAsync();
                var blockInProgress = await dBContext.TokenInfos.MaxAsync(x => x.BlockNumber);
                var lastBlockNumberX16 = await baseScan.GetLastBlockByNumber();
                var lastBlockNumberX10 = Convert.ToInt32(lastBlockNumberX16.result, 16);
                var blockDiff = lastBlockNumberX10 - blockInProgress;

                var text =
                    name +
                    $"{Environment.NewLine} {Environment.NewLine}" +
                    $"DB items today: `{dbTotalCount}` " +
                    $"{Environment.NewLine} {Environment.NewLine}" +
                    $"DB isValid today: `{dbIsValidCount}` " +
                    $"{Environment.NewLine} {Environment.NewLine}" +
                    $"block in progress: `{blockInProgress}` " +
                    $"{Environment.NewLine} {Environment.NewLine}" +
                    $"last block: `{lastBlockNumberX10}` " +
                    $"{Environment.NewLine} {Environment.NewLine}" +
                    $"block diff: `{blockDiff}` " +
                    $"";

                await telegram.SendMessageToGroup(text, optionsTelegram.message_thread_id_healthCheck);
                SetCache();
            }
        }

        public async Task StartNoInfo(string name)
        {
            var isMessageSentToBot = GetCache();
            var sholdSendMessage = DateTime.UtcNow.Minute % 10 == 0;

            if (!isMessageSentToBot && sholdSendMessage)
            {
                var text = name;

                await telegram.SendMessageToGroup(text, optionsTelegram.message_thread_id_healthCheck);
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
