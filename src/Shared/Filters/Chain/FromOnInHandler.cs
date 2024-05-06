using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    /// <summary>
    /// Check if IN transaction exists and go to the FROM address and get data from BaseScan
    /// </summary>
    public class FromOnInHandler : AbstractHandler
    {
        private readonly BaseScan.BaseScanApiClient baseScan;
        public FromOnInHandler(BaseScan.BaseScanApiClient baseScan)
        {
            this.baseScan = baseScan;
        }
        public async override Task<AddressRequest> Handle(AddressRequest request)
        {
            if (request.IsValid)
            {
                request.IsValid = await IsValid(request);
            }

            return await base.Handle(request);

        }
        private async Task<bool> IsValid(AddressRequest request)
        {
            var res = true;

            var vals = request.AddressModel.result;
            var address = request.TokenInfo.AddressOwnersWallet;

            var transInIndex = vals.FindIndex(x => x.to.Equals(address, StringComparison.InvariantCultureIgnoreCase) &&
                                                  !x.from.Equals(address, StringComparison.InvariantCultureIgnoreCase));

            if (transInIndex >= 0)
            {
                var fromAdress = vals[transInIndex].from;
                request.AddressModel = await baseScan.GetInfoByAddress(fromAdress);
            }

            return res;
        }
    }
}
