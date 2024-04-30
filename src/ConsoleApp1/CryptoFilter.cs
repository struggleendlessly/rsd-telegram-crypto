using ConsoleApp1.BaseScanModels;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;

namespace ConsoleApp1
{
    public class CryptoFilter
    {
        public async Task Start()
        {
            var timeHandler = new TimeHandler();
            var removeLiquidityHandler = new RemoveLiquidityHandler();

            timeHandler.SetNext(removeLiquidityHandler);

            List<TokenInfo> process1 = await GetRecordForProcessing();
            List<AddressRequest> processed1 = new();

            foreach (var item in process1)
            {
                var addressRequest = new AddressRequest();
                addressRequest.TokenInfo = item;
                addressRequest.AddressModel = await new BaseScan().GetInfoByAddress(item.AddressOwnersWallet);

                var res = await timeHandler.Handle(addressRequest);
                processed1.Add(addressRequest);
            }

            await UpdateDB(processed1);
        }

        private async Task<int> UpdateDB(List<AddressRequest> collection)
        {
            var res = 0;

            using var db = new DBModel();
            {
                foreach (var item in collection)
                {
                    var token = db.TokenInfos.Where(x => x.Id == item.TokenInfo.Id).FirstOrDefault();

                    if (item.IsValid)
                    {
                        token.IsProcessed1 = true;
                    }
                    else
                    {
                        db.TokenInfos.Remove(token);
                    }
                }

                res = await db.SaveChangesAsync();
            }

            return res;
        }
        private async Task<List<TokenInfo>> GetRecordForProcessing()
        {
            List<TokenInfo> res = new();

            using var db = new DBModel();
            {
                res = await db.
                    TokenInfos.
                    Where(x => x.IsProcessed1 == false).
                    Take(5).
                    ToListAsync();

            }

            return res;
        }
    }
}
