using Microsoft.IdentityModel.Tokens;

using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    /// <summary>
    /// Check the time between Contract created and the next transaction
    /// </summary>
    public class TimeOnContractCreatedHandler : AbstractHandler
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

            var contractCreatedIndex = vals.FindIndex(x => !x.contractAddress.IsNullOrEmpty());

            if (contractCreatedIndex >= 0 && vals.Count > (contractCreatedIndex + 1))
            {
                var timeDiffWithNextTrans = vals[contractCreatedIndex + 1].timeStamp - vals[contractCreatedIndex].timeStamp;

                if (timeDiffWithNextTrans > 1)
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
