using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;

using Microsoft.Extensions.Logging;

using nethereum;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eth_shared
{
    public class GetSwapEvents
    {
        private readonly ILogger logger;
        private readonly ApiWeb3 ApiWeb3;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;
        public GetSwapEvents(
            ILogger<GetSwapEvents> logger,
            ApiWeb3 ApiWeb3,
            EthApi apiAlchemy,
            dbContext dbContext
            )
        {
            this.logger = logger;
            this.ApiWeb3 = ApiWeb3;
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;

        }

        public async Task Start()
        {
            //var lastEthBlockNumber = await apiAlchemy.lastBlockNumber();
            //var lastProcessedBlock = await dbContext.Blocks.MaxAsync(x => x.BlockNumber);

            //var unfiltered = await Get(lastEthBlockNumber, lastProcessedBlock);
            //var validated = Validate(unfiltered);
            //var res = Filter(validated);

            //await SaveToDB(res);
        }

        //public async Task<List<getSwapDTO>> Get(
        //    int lastEthBlockNumber, 
        //    int lastProcessedBlock)
        //{
        //    List<getSwapDTO> res = new();
        //    var block = await apiAlchemy.getSwapLogs(lastEthBlockNumber);
        //    res.Add(block);

        //    return res;
        //}
    }
}
