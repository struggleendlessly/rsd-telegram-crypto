using Microsoft.EntityFrameworkCore;

using Shared.DB;
using Shared.Filters.Chain;
using Shared.Filters.Model;

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
            var timeHandler = new TimeHandler();
            var removeLiquidityHandler = new RemoveLiquidityHandler();

            timeHandler.SetNext(removeLiquidityHandler);

            List<TokenInfo> process1 = await GetRecordForProcessing();
            List<AddressRequest> processed1 = new();

            foreach (var item in process1)
            {
                var addressRequest = new AddressRequest();
                addressRequest.TokenInfo = item;
                addressRequest.AddressModel = await new BaseScan.BaseScan().GetInfoByAddress(item.AddressOwnersWallet);

                var filtered = await timeHandler.Handle(addressRequest);
                processed1.Add(filtered);
            }

            var res = await UpdateDB(processed1, 1);
        }

        private async Task<int> UpdateDB(List<AddressRequest> collection, int processStep)
        {
            var res = 0;

            foreach (var item in collection)
            {
                var token = dBContext.TokenInfos.Where(x => x.Id == item.TokenInfo.Id).FirstOrDefault();
                token.IsValid = item.IsValid;

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
        private async Task<List<TokenInfo>> GetRecordForProcessing()
        {
            List<TokenInfo> res = new();

            res = await dBContext.
                TokenInfos.
                Where(x => x.IsProcessed1 == false).
                Take(5).
                ToListAsync();

            return res;
        }
    }
}
