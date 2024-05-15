using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;
using Shared.DB;
using Shared.Filters.Chain;
using Shared.Filters.Model;

namespace Shared.Filters
{
    public class CryptoFilterProcess1
    {
        private readonly DBContext dBContext;
        private readonly BaseScan.BaseScanApiClient baseScan;
        private readonly Telegram.Telegram telegram;
        private readonly OptionsBanAddresses optionsBanAddresses;
        private readonly OptionsTelegram optionsTelegram;
        public CryptoFilterProcess1(
            DBContext dBContext,
            BaseScan.BaseScanApiClient baseScan,
            IOptions<OptionsTelegram> optionsTelegram,
            Telegram.Telegram telegram,
            IOptions<OptionsBanAddresses> optionsBanAddresses)
        {
            this.dBContext = dBContext;
            this.baseScan = baseScan;
            this.telegram = telegram;
            this.optionsBanAddresses = optionsBanAddresses.Value;
            this.optionsTelegram = optionsTelegram.Value;
        }

        public async Task Start()
        {
            await Process1();
        }

        private async Task Process1()
        {
            List<TokenInfo> to_Process1 = await GetRecordForProcessing1();

            var timeHandler_Process1 = new TimeOnInHandler();
            var checkEmptyTokenAddressHandler_Process1 = new CheckEmptyTokenAddressHandler();
            var checkTheNameOfTokenHandler_Process1 = new CheckTheNameOfTokenHandler();
            var timeOnContractCreatedHandler_Process1 = new TimeOnContractCreatedHandler();
            var checkAmountOfContractsCreatedHandler_Process1 = new CheckAmountOfContractsCreatedHandler();
            var removeLiquidityHandler_Process1 = new RemoveLiquidityHandler();
            var removeLiquidityHandler_Process1_2 = new RemoveLiquidityHandler();
            var fromOnInHandler_Process1 = new FromOnInHandler(baseScan, optionsBanAddresses);
            var checkAmountOfTarnsactionsHandler_Process1 = new CheckAmountOfTarnsactionsHandler();
            var checkTotalSupplyHandler_Process1 = new CheckTotalSupplyHandler(baseScan);
            var checkContractSourceCodeHandler_Process1 = new CheckContractSourceCodeHandler(baseScan);
            var checkFromForManySameAmountsHandler_Process1 = new CheckFromForManySameAmountsHandler();

            timeHandler_Process1.
                //SetNext(checkTheNameOfTokenHandler_Process1).
                SetNext(checkAmountOfContractsCreatedHandler_Process1).
                SetNext(removeLiquidityHandler_Process1).
                SetNext(checkEmptyTokenAddressHandler_Process1).
                SetNext(timeOnContractCreatedHandler_Process1).
                SetNext(checkTotalSupplyHandler_Process1).
                SetNext(checkContractSourceCodeHandler_Process1).
                SetNext(fromOnInHandler_Process1).
                SetNext(checkAmountOfTarnsactionsHandler_Process1).
                SetNext(checkFromForManySameAmountsHandler_Process1).
                SetNext(removeLiquidityHandler_Process1_2);

            var processed1 = await Process(to_Process1, timeHandler_Process1);
            var resProcessed1 = await UpdateDB(processed1, 1);

            foreach (var item in processed1)
            {
                if (item.IsValid)
                {
                    var lastBlockNumberX16 = await baseScan.GetLastBlockByNumber();
                    var lastBlockNumberX10 = Convert.ToInt32(lastBlockNumberX16.result, 16);

                    var text =
                        $"" +
                        $"`{item.TokenInfo.AddressToken}` \n" +

                        $"DB: `{item.TokenInfo.Id}` | " +
                        $"{lastBlockNumberX10 - item.TokenInfo.BlockNumber} 🆗 \n" +

                        $"[Owner]({item.TokenInfo.UrlOwnersWallet}) | " +
                        $"[Token]({item.TokenInfo.UrlToken}) | " +
                        $"[DexScreener]({item.TokenInfo.UrlChart})" +
                        $"";

                    var telMessageId = await telegram.SendMessageToGroup(text, optionsTelegram.message_thread_id_mainfilters);
                    item.TokenInfo.TellMessageIdIsValid = telMessageId;
                }
            }

            var mesIdupdated = UpdateDBTelMessageId(processed1);
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
                token.TellMessageIdIsValid = item.TokenInfo.TellMessageIdIsValid;
            }

            res = await dBContext.SaveChangesAsync();

            return res;
        }

        private async Task<List<TokenInfo>> GetRecordForProcessing1()
        {
            List<TokenInfo> res = new();

            res = await dBContext.
                TokenInfos.
                Where(x => x.IsProcessed1 == false).
                Where(x => x.TimeAdded < DateTime.UtcNow.AddMinutes(-2)).
                //Where(x => x.Id == 17279).
                Take(1).
                ToListAsync();

            return res;
        }
    }
}
