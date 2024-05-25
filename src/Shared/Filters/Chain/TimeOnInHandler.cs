using Microsoft.IdentityModel.Tokens;

using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    /// <summary>
    /// Check the time between IN and the next transaction
    /// </summary>
    public class TimeOnInHandler : AbstractHandler
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

            var vals = request.AddressModel.result;
            var address = request.TokenInfo.AddressOwnersWallet;

            var transInIndex = vals.FindIndex(x => x.contractAddress.IsNullOrEmpty() &&
                                                  !x.from.Equals(address, StringComparison.InvariantCultureIgnoreCase));

            if (transInIndex >= 0 && vals.Count > (transInIndex + 1))
            {
                var timeDiffWithNextTrans = vals[transInIndex + 1].timeStamp - vals[transInIndex].timeStamp;

                if (timeDiffWithNextTrans > 40)
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
