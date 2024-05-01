using Microsoft.EntityFrameworkCore;

using Shared.DB;
using Shared.Filters.Chain;
using Shared.Filters.Model;

using System.Diagnostics;

namespace Shared.Filters
{
    public class CryptoFilter
    {
        private readonly DBContext dBContext;
        public CryptoFilter(DBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public async Task Start()
        {
            List<TokenInfo> toProcess1 = await GetRecordForProcessing1();

            var timeHandlerProcess1 = new TimeHandler();
            var removeLiquidityHandler = new RemoveLiquidityHandler();

            timeHandlerProcess1.SetNext(removeLiquidityHandler);

            var processed1 = await Process(toProcess1, timeHandlerProcess1);
            var resProcessed1 = await UpdateDB(processed1, 1);

            // Process2
            List<TokenInfo> toProcess2 = await GetRecordForProcessing2();

            var removeLiquidityHandlerProcess2 = new RemoveLiquidityHandler();

            var processed2 = await Process(toProcess2, removeLiquidityHandlerProcess2);
            var resProcessed2 = await UpdateDB(processed2, 2);
        }

        public async Task<List<AddressRequest>> Process(List<TokenInfo> toProcess, AbstractHandler handler)
        {           
            List<AddressRequest> res = new();

            foreach (var item in toProcess)
            {
                var addressRequest = new AddressRequest();
                addressRequest.TokenInfo = item;
                addressRequest.AddressModel = await new BaseScan.BaseScan().GetInfoByAddress(item.AddressOwnersWallet);

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
                Take(5).
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
                Take(5).
                ToListAsync();

            return res;
        }
    }
}
