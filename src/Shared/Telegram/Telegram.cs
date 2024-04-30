using Microsoft.EntityFrameworkCore;

using Shared.DB;

using TL;

namespace Shared.Telegram
{
    public class Telegram
    {
        private readonly DBContext dBContext;
        public Telegram(DBContext dBContext)
        {
            this.dBContext = dBContext;
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
            InputPeer peerBaseNewToken = chats.chats[chatBaseNewTokenId];

            List<TokenInfo> res = new();

            for (int offset_id = 0; ;)
            {
                var messages = await client.Messages_GetHistory(peerBaseNewToken, offset_id, default, 0, int.MaxValue, 0, min_id);

                if (messages.Messages.Length == 0) break;

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
                }
            }

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

        static string Config(string what)
        {
            switch (what)
            {
                case "api_id": return "23139632";
                case "api_hash": return "248945b6004da3e679fc64919f571c1e";
                case "phone_number": return "+380689446698";
                case "server_address": return "2>149.154.167.50:443";
                case "verification_code": Console.Write("Code: "); return Console.ReadLine();
                case "first_name": return "John";      // if sign-up is required
                case "last_name": return "Doe";        // if sign-up is required
                case "password": return "secret!";     // if user has enabled 2FA
                default: return null;                  // let WTelegramClient decide the default config
            }
        }
    }
}
