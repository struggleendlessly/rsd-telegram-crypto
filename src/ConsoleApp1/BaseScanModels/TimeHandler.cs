namespace ConsoleApp1.BaseScanModels
{
    public class TimeHandler : AbstractHandler
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

            var transInIndex = vals.FindIndex(x => x.to.Equals(address, StringComparison.InvariantCultureIgnoreCase) &&
                                                  !x.from.Equals(address, StringComparison.InvariantCultureIgnoreCase));

            if (transInIndex >= 0)
            {
                var timeDiffWithNextTrans = vals[transInIndex + 1].timeStamp - vals[transInIndex].timeStamp;

                if (timeDiffWithNextTrans > 120)
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
