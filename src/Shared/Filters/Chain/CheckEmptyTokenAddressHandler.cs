using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    /// <summary>
    /// Check if IN transaction exists and go to the FROM address and get data from BaseScan
    /// </summary>
    public class CheckEmptyTokenAddressHandler : AbstractHandler
    {
        public CheckEmptyTokenAddressHandler()
        {
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

            if (string.IsNullOrEmpty(contractAddress))
            {
                res = true;
            }

            return res;
        }
    }
}
