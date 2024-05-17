using Microsoft.IdentityModel.Tokens;

using Shared.Filters.Model;

using System.Diagnostics.Contracts;
using System.Net;
using System.Text.RegularExpressions;

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

                var soursceCode = contractSourceCode.result[0].SourceCode;

                var addBotContains =
                        soursceCode.
                        Contains("addbot", StringComparison.InvariantCultureIgnoreCase);

                var addB0tContains =
                        soursceCode.
                        Contains("addb0t", StringComparison.InvariantCultureIgnoreCase);

                var swapTokensForEthContains =
                        soursceCode.
                        Contains("swapTokensForEth", StringComparison.InvariantCultureIgnoreCase);

                string patternInit = @"\bfunction init\b";
                bool initContains = Regex.IsMatch(soursceCode, patternInit, RegexOptions.IgnoreCase);

                if (addBotContains ||
                    addB0tContains ||
                    initContains)
                {
                    isValid = false;
                }

                // swapTokensForEth should just left in isValid and not show in other chats 
                if (swapTokensForEthContains)
                {
                    isContractVerified = false;
                }
            }

            request.IsValid = isValid;
            request.isContractVerified = isContractVerified;
        }
    }
}
