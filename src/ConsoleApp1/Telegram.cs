using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TL;

namespace ConsoleApp1
{
    public class Telegram
    {
        public async Task ReadNewMessages()
        {
            using var client = new WTelegram.Client(Config);
            var myself = await client.LoginUserIfNeeded();

            var chats = await client.Messages_GetAllChats();
            int chatBaseNewTokenId = 1958915778;
            InputPeer peerBaseNewToken = chats.chats[chatBaseNewTokenId];

            using var db = new DBModel();
            {
                var min_id = db.
                    TokenInfos.
                    OrderByDescending(x => x.TelegramMessageId).
                    FirstOrDefault()?.
                    TelegramMessageId ?? 0;

                var res = new List<TokenInfo>();

                for (int offset_id = 0; ;)
                {
                    var messages = await client.Messages_GetHistory(peerBaseNewToken, offset_id, default, 0, int.MaxValue, 0, min_id);

                    if (messages.Messages.Length == 0) break;
                    
                    foreach (var msgBase in messages.Messages)
                    {
                        //var from = messages.UserOrChat(msgBase.From ?? msgBase.Peer); // from can be User/Chat/Channel
                        if (msgBase is Message msg)
                        {
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

                            res.Add(tokenInfo);
                            Console.WriteLine($"> {msg.message} {msg.media}");
                            Console.WriteLine(Environment.NewLine);
                        }
                    }

                    db.TokenInfos.AddRange(res);
                    db.SaveChanges();
                    offset_id = messages.Messages[^1].ID;
                }
            }
        }
        static string Config(string what)
        {
            switch (what)
            {
                case "api_id": return "23880234";
                case "api_hash": return "c349bbf6c93c7df984219a77aeb320df";
                case "phone_number": return "+380996000291";
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
