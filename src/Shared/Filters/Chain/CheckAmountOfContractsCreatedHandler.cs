using Microsoft.IdentityModel.Tokens;

using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    /// <summary>
    /// Check if the amount of Contracts created is less or equal  4
    /// </summary>
    public class CheckAmountOfContractsCreatedHandler : AbstractHandler
    {
        public async override Task<AddressRequest> Handle(AddressRequest request)
        {
            Console.WriteLine(GetType().Name);

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

            var amountOfContractsCreated = vals.Count(x => !x.contractAddress.IsNullOrEmpty());

            if (amountOfContractsCreated <= 4)
            {
                res = true;
            }

            return res;
        }
    }
}
