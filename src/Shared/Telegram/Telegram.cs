using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;
using Shared.DB;

using TL;

namespace Shared.Telegram
{
    public class Telegram
    {
        private readonly DBContext dBContext;
        private readonly OptionsTelegram optionsTelegram;
        public Telegram(DBContext dBContext, IOptions<OptionsTelegram> optionsTelegram)
        {
            this.dBContext = dBContext;
            this.optionsTelegram = optionsTelegram.Value;
        }
        public async Task Start()
        {
            var latestTelegramMessageId = await GetLatestTelegramMessageId();
            var newTelegramMessages = await ReadNewMessages(latestTelegramMessageId);
            var savedToDB = await SaveToDB(newTelegramMessages);
        }

        private async Task<int> GetLatestTelegramMessageId()
        {
            var res = 0;

            var min_id = await dBContext.
                TokenInfos.
                OrderByDescending(x => x.TelegramMessageId).
                FirstOrDefaultAsync();

            res = min_id?.TelegramMessageId ?? 0;

            return res;
        }

        private async Task<int> SaveToDB(List<TokenInfo> val)
        {
            var res = 0;

            await dBContext.TokenInfos.AddRangeAsync(val);

            res = await dBContext.SaveChangesAsync();

            return res;
        }

        private async Task<List<TokenInfo>> ReadNewMessages(int min_id)
        {
            using var client = new WTelegram.Client(Config);
            var myself = await client.LoginUserIfNeeded();

            var chats = await client.Messages_GetAllChats();
            int chatBaseNewTokenId = 1958915778;
            //InputPeer peerBaseNewToken = chats.chats[chatBaseNewTokenId];
            var peerBaseNewToken = new InputPeerChannel ( chatBaseNewTokenId, 3635553435702714717);
            List<TokenInfo> res = new();

            int offset_id = 0;
            //for (int offset_id = 0; ;)
            //{
            var messages = await client.Messages_GetHistory(
                peer: peerBaseNewToken,
                offset_id: offset_id,
                offset_date: default,
                add_offset: 0,
                limit: optionsTelegram.api_limit,
                max_id: 0,
                min_id: min_id);

            if (messages.Messages.Length == 0) return res;

            foreach (MessageBase msgBase in messages.Messages)
            {
                //var from = messages.UserOrChat(msgBase.From ?? msgBase.Peer); // from can be User/Chat/Channel
                if (msgBase is Message msg)
                {
                    var tokenInfo = Map(msg, msgBase);

                    res.Add(tokenInfo);
                    Console.WriteLine($"> {msg.message} {msg.media}");
                    Console.WriteLine(Environment.NewLine);
                }
                await Task.Delay(optionsTelegram.api_delay_forech);
            }

            offset_id = messages.Messages[^1].ID;
            // }

            return res;
        }

        private static TokenInfo Map(Message msg, MessageBase msgBase)
        {
            TokenInfo res = new();

            var entities = msg.entities;

            var tokenInfo = new TokenInfo()
            {
                AddressToken = string.Empty,
                AddressOwnersWallet = string.Empty,

                TelegramMessageId = msgBase.ID,
                TelegramMessage = msg.message,

                UrlToken = (entities[2] as MessageEntityTextUrl).url,
                UrlOwnersWallet = (entities[3] as MessageEntityTextUrl).url,
                UrlChart = (entities[4] as MessageEntityTextUrl).url,

                TimeAdded = DateTime.UtcNow,
                TimeUpdated = DateTime.UtcNow
            };

            var addressToken = tokenInfo.UrlToken.Split("https://basescan.org/token/").Last();
            var addressOwnersWallet = tokenInfo.UrlOwnersWallet.Split("https://basescan.org/address/").Last();

            tokenInfo.AddressToken = addressToken;
            tokenInfo.AddressOwnersWallet = addressOwnersWallet;

            res = tokenInfo;

            return res;
        }

        string Config(string what)
        {
            switch (what)
            {
                case "api_id": return optionsTelegram.api_id;
                case "api_hash": return optionsTelegram.api_hash;
                case "phone_number": return optionsTelegram.phone_number;
                case "server_address": return optionsTelegram.server_address;
                case "verification_code": Console.Write("Code: "); return Console.ReadLine();
                case "session_pathname": return optionsTelegram.session_pathname;
                default: return null;                  // let WTelegramClient decide the default config
            }
        }
    }
}
