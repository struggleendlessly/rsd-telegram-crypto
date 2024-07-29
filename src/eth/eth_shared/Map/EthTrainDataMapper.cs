using api_alchemy.Eth.ResponseDTO;

using Data.Models;

using etherscan.ResponseDTO;

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
        
        public static EthTrainData Map(this EthTrainData  ethTrainData, GetSourceCodeDTO sourceCodeDTO)
        {
            var item = sourceCodeDTO.result.Single();

            ethTrainData.ABI = item.ABI;
            ethTrainData.ContractName = item.ContractName;
            ethTrainData.CompilerVersion = item.CompilerVersion;
            ethTrainData.ConstructorArguments = item.ConstructorArguments;
            ethTrainData.EVMVersion = item.EVMVersion;
            ethTrainData.LicenseType = item.LicenseType;
            ethTrainData.Library = item.Library;
            ethTrainData.OptimizationUsed = item.OptimizationUsed;
            ethTrainData.Proxy = item.Proxy;
            ethTrainData.Runs = item.Runs;
            ethTrainData.SourceCode = item.SourceCode;
            ethTrainData.SwarmSource = item.SwarmSource;
            ethTrainData.Implementation = item.Implementation;

            return ethTrainData;
        }
    }
}
