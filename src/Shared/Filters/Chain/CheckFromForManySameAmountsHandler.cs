using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    /// <summary>
    /// Check ContractSourceCode for the addBot or addB0t method
    /// </summary>
    public class CheckFromForManySameAmountsHandler : AbstractHandler
    {
        public CheckFromForManySameAmountsHandler()
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

            var results = request.AddressModel.result;

            var t = results.
                GroupBy(x => x.value).
                Where(x => x.Count() >= 5).
                ToList();

            if (t.Count == 0)
            {
                res = true;
            }

            return res;
        }
    }
}
