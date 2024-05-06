using Microsoft.EntityFrameworkCore;

using Shared.DB;
using Shared.Filters.Chain;
using Shared.Filters.Model;

namespace Shared.Filters
{
    public class CryptoFilter
    {
        private readonly DBContext dBContext;
        private readonly BaseScan.BaseScanApiClient baseScan;
        private readonly Telegram.Telegram telegram;
        public CryptoFilter(
            DBContext dBContext, 
            BaseScan.BaseScanApiClient baseScan,
            Telegram.Telegram telegram)
        {
            this.dBContext = dBContext;
            this.baseScan = baseScan;
            this.telegram = telegram;
        }

        public async Task Start()
        {
            List<TokenInfo> toProcess1 = await GetRecordForProcessing1();

            var timeHandlerProcess1 = new TimeOnInHandler();
            var checkTheNameOfTokenHandlerProcess1 = new CheckTheNameOfTokenHandler();
            var timeOnContractCreatedHandlerProcess1 = new TimeOnContractCreatedHandler();
            var checkAmountOfContractsCreatedHandlerProcess1 = new CheckAmountOfContractsCreatedHandler();
            var removeLiquidityHandlerProcess1 = new RemoveLiquidityHandler();
            var removeLiquidityHandlerProcess1_2 = new RemoveLiquidityHandler();
            var fromOnInHandlerProcess1 = new FromOnInHandler(baseScan);
            var checkAmountOfTarnsactionsHandlerProcess1 = new CheckAmountOfTarnsactionsHandler();
            var checkTotalSupplyHandlerProcess1 = new CheckTotalSupplyHandler(baseScan);
            var checkContractSourceCodeHandlerProcess1 = new CheckContractSourceCodeHandler(baseScan);

            timeHandlerProcess1.
                //SetNext(checkTheNameOfTokenHandlerProcess1).
                SetNext(checkAmountOfContractsCreatedHandlerProcess1).
                SetNext(timeOnContractCreatedHandlerProcess1).
                SetNext(removeLiquidityHandlerProcess1).
                //SetNext(checkTotalSupplyHandlerProcess1).
                SetNext(checkContractSourceCodeHandlerProcess1).
                SetNext(fromOnInHandlerProcess1).
                SetNext(checkAmountOfTarnsactionsHandlerProcess1).
                SetNext(removeLiquidityHandlerProcess1_2);

            var processed1 = await Process(toProcess1, timeHandlerProcess1);
            var resProcessed1 = await UpdateDB(processed1, 1);

            foreach (var item in processed1)
            {
                if (item.IsValid)
                {
                    await telegram.SendMessageToGroup($"" +
                        $"Found a valid token with info: " +

                        $"{Environment.NewLine} " +
                        $"DB id: {item.TokenInfo.Id} " +

                        $"{Environment.NewLine} " +
                        $"Owner: {item.TokenInfo.UrlOwnersWallet} " +

                        $"{Environment.NewLine} " +
                        $"Chart: {item.TokenInfo.UrlChart}" +

                        $"{Environment.NewLine} " +
                        $"Token: {item.TokenInfo.UrlToken}" +
                        $"");
                }
            }   

            // ----------------------------------------------
            // Process2 - check in 5 hours 
            //List<TokenInfo> toProcess2 = await GetRecordForProcessing2();
            //var fromOnInHandlerProcess2 = new FromOnInHandler(baseScan);
            //var removeLiquidityHandlerProcess2 = new RemoveLiquidityHandler();
            //var removeLiquidityHandlerProcess2_2 = new RemoveLiquidityHandler();

            //removeLiquidityHandlerProcess2.
            //    SetNext(fromOnInHandlerProcess2).
            //    SetNext(removeLiquidityHandlerProcess2_2);

            //var processed2 = await Process(toProcess2, removeLiquidityHandlerProcess2);
            //var resProcessed2 = await UpdateDB(processed2, 2);
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

                            if (!item.IsValid)
                            {
                                token.IsProcessed2 = true;
                            }

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
        private async Task<List<TokenInfo>> GetRecordForProcessing1()
        {
            List<TokenInfo> res = new();

            res = await dBContext.
                TokenInfos.
                Where(x => x.IsProcessed1 == false).
                //Where(x => x.Id == 5715).
                Take(1).
                ToListAsync();

            return res;
        }

        private async Task<List<TokenInfo>> GetRecordForProcessing2()
        {
            List<TokenInfo> res = new();

            res = await dBContext.
                TokenInfos.
                Where(x => x.IsProcessed2 == false && x.IsValid == true).
                Where(x => x.TimeUpdated > DateTime.UtcNow.AddHours(-5)).
                Take(2).
                ToListAsync();

            return res;
        }
    }
}
