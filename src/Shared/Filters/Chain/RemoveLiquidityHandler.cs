using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    /// <summary>
    /// Check if at least one removeLiquidity function is present
    /// </summary>
    public class RemoveLiquidityHandler : AbstractHandler
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

            var isRemoveLiquidity = vals.Any(x => x.functionName.Contains("removeLiquidity", StringComparison.InvariantCultureIgnoreCase));

            if (!isRemoveLiquidity)
            {
                res = true;
            }

            return res;
        }
    }
}
