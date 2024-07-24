using api_alchemy.Eth.ResponseDTO;

using Data.Models;

namespace eth_shared.Map
{
    public static class EthBlocksMapper
    {
        public static EthBlocks Map(this getBlockByNumberDTO t)
        {
            var res = new EthBlocks();

            if (t.result is not null)
            {
                var block = t.result;
                res = new EthBlocks()
                {
                    number = block.number,
                    baseFeePerGas = block.baseFeePerGas,
                    gasLimit = block.gasLimit,
                    gasUsed = block.gasUsed,
                    timestamp = block.timestamp
                };
            }
            return res;
        }
    }
}
