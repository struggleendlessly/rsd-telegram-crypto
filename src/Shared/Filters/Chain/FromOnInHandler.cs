using Shared.ConfigurationOptions;
using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    /// <summary>
    /// Check if IN transaction exists and go to the FROM address and get data from BaseScan
    /// </summary>
    public class FromOnInHandler : AbstractHandler
    {
        private readonly BaseScan.BaseScanApiClient baseScan;
        private readonly OptionsBanAddresses optionsBanAddresses;
        public FromOnInHandler(
            BaseScan.BaseScanApiClient baseScan,
            OptionsBanAddresses optionsBanAddresses)
        {
            this.baseScan = baseScan;
            this.optionsBanAddresses = optionsBanAddresses;
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

                if (optionsBanAddresses.Addresses.Contains(fromAdress))
                {
                    res = false;
                }
                else
                {
                    request.AddressModel = await baseScan.GetInfoByAddress(fromAdress);
                }
            }

            return res;
        }
    }
}
