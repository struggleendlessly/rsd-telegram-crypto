using Microsoft.IdentityModel.Tokens;

using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    /// <summary>
    /// Check if the name of the token is valid (not empty or does not contain .)
    /// </summary>
    public class CheckTheNameOfTokenHandler : AbstractHandler
    {
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

            var tokenName = request.TokenInfo.NameToken;

            if (!tokenName.Contains(".") &&
                !tokenName.Contains("test", StringComparison.InvariantCultureIgnoreCase) &&
                !tokenName.Contains("()"))
            {
                res = true;
            }

            return res;
        }
    }
}
