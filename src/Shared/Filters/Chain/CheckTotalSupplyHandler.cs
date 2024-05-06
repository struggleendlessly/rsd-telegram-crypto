using Microsoft.IdentityModel.Tokens;

using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    /// <summary>
    /// Check total supply of the token more then 1_000_000
    /// </summary>
    public class CheckTotalSupplyHandler : AbstractHandler
    {
        private readonly BaseScan.BaseScanApiClient baseScan;
        public CheckTotalSupplyHandler(BaseScan.BaseScanApiClient baseScan)
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
            var res = false;

            var contractAddress = request.TokenInfo.HashToken;
            var tatalSupply = await baseScan.GetTotalSupply(contractAddress);
            var tatalSupplyAmountString = "";

            if (tatalSupply.result.Length > 18)
            {
                tatalSupplyAmountString = tatalSupply.result.Remove(tatalSupply.result.Length - 18);
            }

            var isParced = ulong.TryParse(tatalSupplyAmountString, out ulong tatalSupplyAmountNumber);

            if (isParced == true && tatalSupplyAmountNumber >= 1_000_000)
            {
                res = true;
            }

            return res;
        }
    }
}
