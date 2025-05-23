﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Shared.BaseScan;
using Shared.ConfigurationOptions;
using Shared.DB;
using Shared.Filters.Chain;
using Shared.Filters.Model;

namespace Shared.Filters
{
    public class CryptoFilterProcess2
    {
        private readonly DBContext dBContext;
        private readonly BaseScan.BaseScanApiClient baseScan;
        private readonly Telegram.Telegram telegram;
        private readonly OptionsBanAddresses optionsBanAddresses;
        private readonly OptionsTelegram optionsTelegram;
        private readonly BaseScanContractScraper baseScanContractScraper;
        public CryptoFilterProcess2(
            DBContext dBContext,
            BaseScan.BaseScanApiClient baseScan,
            IOptions<OptionsTelegram> optionsTelegram,
            Telegram.Telegram telegram,
            IOptions<OptionsBanAddresses> optionsBanAddresses,
            BaseScanContractScraper baseScanContractScraper)
        {
            this.dBContext = dBContext;
            this.baseScan = baseScan;
            this.telegram = telegram;
            this.optionsBanAddresses = optionsBanAddresses.Value;
            this.optionsTelegram = optionsTelegram.Value;
            this.baseScanContractScraper = baseScanContractScraper;

            baseScan.SetApiKeyToken(2);
        }

        public async Task Start()
        {
            await Process2();
        }

        public async Task Process2()
        {
            List<TokenInfo> to_Process = await GetRecordForProcessing();

            var checkContractSourceCodeProcess2Handler = new CheckContractSourceCodeProcess2Handler(baseScan);
            var processed = await Process2(to_Process, checkContractSourceCodeProcess2Handler);
            var resProcessed = await UpdateDB(processed, 2);

            foreach (var item in processed)
            {
                if (item.IsValid)
                {
                    if (item.isContractVerified)
                    {
                        if (item.TokenInfo.TellMessageIdBotVerified != 0)
                        {
                            continue;
                        }

                        var text = await PrepareTextForTelegram(item.TokenInfo, "✅");

                        var telMessageId = await telegram.SendMessageToGroup(text, optionsTelegram.message_thread_id_botVerified);
                        item.TokenInfo.TellMessageIdBotVerified = telMessageId;

                        if (item.TokenInfo.TellMessageIdNotVerified != 0)
                        {
                            telMessageId = await telegram.DeleteMessageInGroup(item.TokenInfo.TellMessageIdNotVerified, optionsTelegram.message_thread_id_unVerified);
                        }

                        var a = await baseScanContractScraper.SaveContractSourceCodeToDB(item.TokenInfo.AddressToken);
                    }
                    else
                    {
                        if (item.TokenInfo.TellMessageIdNotVerified != 0)
                        {
                            continue;
                        }

                        var text = await PrepareTextForTelegram(item.TokenInfo, "🚫");

                        var telMessageId = await telegram.SendMessageToGroup(text, optionsTelegram.message_thread_id_unVerified);
                        item.TokenInfo.TellMessageIdNotVerified = telMessageId;
                    }
                }
                else
                {
                    var telMessageId = await telegram.DeleteMessageInGroup(item.TokenInfo.TellMessageIdIsValid, optionsTelegram.message_thread_id_mainfilters);

                    if (item.TokenInfo.TellMessageIdNotVerified != 0)
                    {
                        telMessageId = await telegram.DeleteMessageInGroup(item.TokenInfo.TellMessageIdNotVerified, optionsTelegram.message_thread_id_unVerified);
                    }
                }
            }

            var mesIdupdated = UpdateDBTelMessageId(processed);
        }

        private async Task<string> PrepareTextForTelegram(TokenInfo tokenInfo, string icon)
        {
            var res = "";

            var lastBlockNumberX16 = await baseScan.GetLastBlockByNumber();
            var lastBlockNumberX10 = Convert.ToInt32(lastBlockNumberX16.result, 16);

            res =
                $"" +
                $"`{tokenInfo.AddressToken}` \n" +

                $"Name: `{tokenInfo.NameToken}` | " +
                $"DB: `{tokenInfo.Id}` | " +
                $"{lastBlockNumberX10 - tokenInfo.BlockNumber} {icon} \n" +

                $"Supply: `{tokenInfo.totalSupply}` | " +
                $"Divisor: `{tokenInfo.divisor}` | \n" +

                $"[Owner]({tokenInfo.UrlOwnersWallet}) | " +
                $"[Token]({tokenInfo.UrlToken}) | " +
                $"[DexScreener]({tokenInfo.UrlChart})" +
                $"";

            return res;
        }
        public async Task<List<AddressRequest>> Process2(List<TokenInfo> toProcess, AbstractHandler handler)
        {
            List<AddressRequest> res = new();

            foreach (var item in toProcess)
            {
                Console.WriteLine($"Processing {item.AddressOwnersWallet}");

                var addressRequest = new AddressRequest();
                addressRequest.TokenInfo = item;
                addressRequest.TokenInfo.IsValid = true;
                addressRequest.TokenInfo.ErrorType = "";
                addressRequest.AddressModel = await baseScan.GetInfoByAddress(item.AddressOwnersWallet);

                var filtered = await handler.Handle(addressRequest);
                res.Add(filtered);

                await Task.Delay(250);
            }

            return res;
        }

        public async Task<List<AddressRequest>> Process(List<TokenInfo> toProcess, AbstractHandler handler)
        {
            List<AddressRequest> res = new();

            foreach (var item in toProcess)
            {
                Console.WriteLine($"Processing {item.AddressOwnersWallet}");

                var addressRequest = new AddressRequest();
                addressRequest.TokenInfo = item;
                addressRequest.TokenInfo.IsValid = true;
                addressRequest.TokenInfo.ErrorType = "";
                addressRequest.AddressModel = await baseScan.GetInfoByAddress(item.AddressOwnersWallet);

                var filtered = await handler.Handle(addressRequest);
                res.Add(filtered);
            }

            return res;
        }


        private async Task<int> UpdateDB(List<AddressRequest> collection, int processStep)
        {
            var res = 0;

            foreach (var item in collection)
            {
                var token = dBContext.TokenInfos.Where(x => x.Id == item.TokenInfo.Id).FirstOrDefault();
                token.IsValid = item.IsValid;
                token.ErrorType = item.TokenInfo.ErrorType;
                token.TimeUpdated = DateTime.UtcNow;

                await baseScanContractScraper.UpdateDBWithPaidApiTokenInfo(token);

                switch (processStep)
                {
                    case 1:
                        {
                            token.IsProcessed1 = true;
                            break;
                        }
                    case 2:
                        token.IsProcessed2 = true;
                        break;
                    default:
                        break;
                }
            }

            res = await dBContext.SaveChangesAsync();

            return res;
        }

        private async Task<int> UpdateDBTelMessageId(List<AddressRequest> collection)
        {
            var res = 0;

            foreach (var item in collection)
            {
                var token = dBContext.TokenInfos.Where(x => x.Id == item.TokenInfo.Id).FirstOrDefault();
                token.TellMessageIdBotVerified = item.TokenInfo.TellMessageIdBotVerified;
                token.TellMessageIdNotVerified = item.TokenInfo.TellMessageIdNotVerified;
            }

            res = await dBContext.SaveChangesAsync();

            return res;
        }

        private async Task<List<TokenInfo>> GetRecordForProcessing()
        {
            List<TokenInfo> res = new();

            res = await dBContext.
                TokenInfos.
                Where(x => x.IsValid == true && !string.IsNullOrEmpty(x.AddressToken) && x.TellMessageIdBotVerified == 0).
                Where(x => x.TimeUpdated > DateTime.UtcNow.AddHours(-48)).
                //Where(x => x.Id == 139574).
                //Take(2).
                ToListAsync();

            return res;
        }
    }
}
