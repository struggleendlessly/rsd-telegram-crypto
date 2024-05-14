using Microsoft.IdentityModel.Tokens;

using Shared.Filters.Model;

using System.Net;

namespace Shared.Filters.Chain
{
    /// <summary>
    /// Check ContractSourceCode for the addBot or addB0t method
    /// </summary>
    public class CheckContractSourceCodeProcess2Handler : AbstractHandler
    {
        private readonly BaseScan.BaseScanApiClient baseScan;
        public CheckContractSourceCodeProcess2Handler(BaseScan.BaseScanApiClient baseScan)
        {
            this.baseScan = baseScan;
        }

        public async override Task<AddressRequest> Handle(AddressRequest request)
        {
            if (request.IsValid)
            {
                await IsValid(request);
            }

            return await base.Handle(request);

        }

        private async Task IsValid(AddressRequest request)
        {
            var isValid = true;
            var isContractVerified = false;

            var contractAddress = request.TokenInfo.AddressToken;

            var contractSourceCode = await baseScan.GetContractSourceCode(contractAddress);

            if (!contractSourceCode.result[0].SourceCode.IsNullOrEmpty())
            {
                isContractVerified = true;

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

                if (addBotContains || addB0tContains)
                {
                    isValid = false;
                }
            }

            request.IsValid = isValid;
            request.isContractVerified = isContractVerified;
        }
    }
}
