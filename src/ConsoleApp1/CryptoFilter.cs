using ConsoleApp1.BaseScanModels;

namespace ConsoleApp1
{
    public class CryptoFilter
    {
        public async Task<bool> IsTimeValid(List<Result> vals, string address)
        {
            var res = false;

            var transInIndex = vals.FindIndex(x => x.to.Equals(address, StringComparison.InvariantCultureIgnoreCase) &&
                                                  !x.from.Equals(address, StringComparison.InvariantCultureIgnoreCase));

            var timeDiffWithNextTrans = vals[transInIndex + 1].timeStamp - vals[transInIndex].timeStamp;

            if (timeDiffWithNextTrans > 120)
            {
                res = true;
            }

            return res;
        }
    }
}
