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
            List<TokenInfo> to_Process1 = await GetRecordForProcessing1();

            var timeHandler_Process1 = new TimeOnInHandler();
            var checkEmptyTokenAddressHandler_Process1 = new CheckEmptyTokenAddressHandler();
            var checkTheNameOfTokenHandler_Process1 = new CheckTheNameOfTokenHandler();
            var timeOnContractCreatedHandler_Process1 = new TimeOnContractCreatedHandler();
            var checkAmountOfContractsCreatedHandler_Process1 = new CheckAmountOfContractsCreatedHandler();
            var removeLiquidityHandler_Process1 = new RemoveLiquidityHandler();
            var removeLiquidityHandler_Process1_2 = new RemoveLiquidityHandler();
            var fromOnInHandler_Process1 = new FromOnInHandler(baseScan);
            var checkAmountOfTarnsactionsHandler_Process1 = new CheckAmountOfTarnsactionsHandler();
            var checkTotalSupplyHandler_Process1 = new CheckTotalSupplyHandler(baseScan);
            var checkContractSourceCodeHandler_Process1 = new CheckContractSourceCodeHandler(baseScan);

            timeHandler_Process1.
                //SetNext(checkTheNameOfTokenHandler_Process1).
                SetNext(checkEmptyTokenAddressHandler_Process1).
                SetNext(checkAmountOfContractsCreatedHandler_Process1).
                SetNext(timeOnContractCreatedHandler_Process1).
                SetNext(removeLiquidityHandler_Process1).
                SetNext(checkTotalSupplyHandler_Process1).
                SetNext(checkContractSourceCodeHandler_Process1).
                SetNext(fromOnInHandler_Process1).
                SetNext(checkAmountOfTarnsactionsHandler_Process1).
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
                        $"`{item.TokenInfo.AddressToken}`" +

                        $"{Environment.NewLine} {Environment.NewLine}" +
                        $"DB: `{item.TokenInfo.Id}` | " +
                        $"`{item.TokenInfo.BlockNumber}` | " +
                        $"`{lastBlockNumberX10}` | " +
                        $"{lastBlockNumberX10 - item.TokenInfo.BlockNumber} " +

                        $"{Environment.NewLine} {Environment.NewLine} " +
                        $"[Owner]({item.TokenInfo.UrlOwnersWallet}) | " +
                        $"[Token]({item.TokenInfo.UrlToken}) | " +
                        $"[DexScreener]({item.TokenInfo.UrlChart})" +
                        $"";

                    await telegram.SendMessageToGroup(text);
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
                //Where(x => x.IsProcessed1 == false).
                //Where(x => x.TimeAdded < DateTime.UtcNow.AddMinutes(-2)).
                Where(x => x.Id == 11178).
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
