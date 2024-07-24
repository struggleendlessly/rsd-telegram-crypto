using api_alchemy.Eth.ResponseDTO;

using Data.Models;

namespace eth_shared.Map
{
    public static class EthTrainDataMapper
    {
        public static EthTrainData Map(this Transaction t)
        {
            var res = new EthTrainData()
            {
                blockHash = t.blockHash,
                blockNumber = t.blockNumber,
                from = t.from,
                gas = t.gas,
                gasPrice = t.gasPrice,
                hash = t.hash,
                input = t.input,
                nonce = t.nonce,
                to = t.to,
                transactionIndex = t.transactionIndex,
                value = t.value,
                v = t.v,
                r = t.r,
                s = t.s,
                chainId = t.chainId,
                maxFeePerBlobGas = t.maxFeePerBlobGas,
                maxFeePerGas = t.maxFeePerGas,
                maxPriorityFeePerGas = t.maxPriorityFeePerGas,
                yParity = t.yParity,
                isCustomInputStart = false,
            };

            return res;
        }
    }
}
