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

            var contractAddress = request.TokenInfo.AddressToken;
            var tatalSupply = request.TokenInfo.totalSupply;
            var divisor = 0;           

            var tatalSupplyAmountString = "";

            if (string.IsNullOrEmpty(tatalSupply))
            {
                res = true;
            }

            var isParcedDivisor = int.TryParse(request.TokenInfo.divisor, out divisor);

            if (isParcedDivisor == true && tatalSupply.Length > divisor)
            {
                tatalSupplyAmountString = tatalSupply.Remove(tatalSupply.Length - divisor);
            }

            var isParcedSupply = ulong.TryParse(tatalSupplyAmountString, out ulong tatalSupplyAmountNumber);

            if (isParcedSupply == true && tatalSupplyAmountNumber >= 1_000_000)
            {
                res = true;
            }

            return res;
        }
    }
}
