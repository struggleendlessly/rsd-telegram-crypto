using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    public class CheckAmountOfTarnsactionsHandler : AbstractHandler
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

            if (vals.Count >= 2)
            {
                res = true;
            }

            return res;
        }
    }
}
