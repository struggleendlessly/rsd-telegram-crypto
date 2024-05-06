using Microsoft.IdentityModel.Tokens;

using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    /// <summary>
    /// Check ContractSourceCode for the addBot or addB0t method
    /// </summary>
    public class CheckContractSourceCodeHandler : AbstractHandler
    {
        private readonly BaseScan.BaseScanApiClient baseScan;
        public CheckContractSourceCodeHandler(BaseScan.BaseScanApiClient baseScan)
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
            var contractSourceCode = await baseScan.GetContractSourceCode(contractAddress);

            if (!contractSourceCode.result[0].SourceCode.IsNullOrEmpty())
            {
                var addBotContains =
                        contractSourceCode.
                        result[0].
                        SourceCode.
                        Contains("addbot", StringComparison.InvariantCultureIgnoreCase);

                var addB0tContains =
                        contractSourceCode.
                        result[0].
                        SourceCode.
                        Contains("addb0t", StringComparison.InvariantCultureIgnoreCase);

                if (!addBotContains && !addB0tContains)
                {
                    res = true;
                }
            }
            else
            {
                res = true;
            }

            return res;
        }
    }
}
