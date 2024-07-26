using api_alchemy.Eth.ResponseDTO;

using System.Collections.Concurrent;

namespace eth_shared.Filters
{
    public class FilterBlock
    {

        public static List<getBlockByNumberDTO> Filter_EmptyBlocks_Distinct(
            ConcurrentBag<getBlockByNumberDTO> blocksUnfiltered)
        {
            List<getBlockByNumberDTO> res = new();

            foreach (var item in blocksUnfiltered)
            {
                if (item.result is not null &&
                    item.result.transactions is not null)
                {
                    res.Add(item);
                }
            }

            res = res.DistinctBy(x => x.result.number).ToList();

            return res;
        }
    }
}
