using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using nethereum;

namespace eth_shared
{
    public class GetSwapEvents
    {
        private readonly ILogger logger;
        private readonly ApiWeb3 ApiWeb3;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;

        int lastEthBlockNumber = 0;
        int lastProcessedBlock = 18911035;
        int lastBlockToProcess = 0;

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
            while (true)
            {


                lastEthBlockNumber = await apiAlchemy.lastBlockNumber();

                if (await dbContext.EthSwapEvents.AnyAsync())
                {
                    lastProcessedBlock = await dbContext.EthSwapEvents.MaxAsync(x => x.blockNumberInt);
                }

                lastBlockToProcess = lastProcessedBlock + 2000;

                if (lastBlockToProcess > lastEthBlockNumber)
                {
                    lastBlockToProcess = lastEthBlockNumber;
                }

                var tokensToProcess = await GetTokensToProcess();
                var unfiltered = await Get(tokensToProcess);
                var validated = Validate(unfiltered);

                if (validated.Count == 0)
                {
                    var t = new EthSwapEvents();
                    t.blockNumberInt = lastBlockToProcess;

                    dbContext.EthSwapEvents.Add(t);
                    // await dbContext.SaveChangesAsync();
                }
                else
                {

                }
                var logs = validated.FirstOrDefault().result;
                var logsJson = System.Text.Json.JsonSerializer.Serialize(logs);
                var ee = ApiWeb3.DecodeSwapEvents(logsJson);
            }
            //var validated = Validate(unfiltered);
            //var res = Filter(validated);

            //await SaveToDB(res);
        }

        public List<getSwapDTO> Validate(List<getSwapDTO> collection)
        {
            List<getSwapDTO> res = new();

            foreach (var item in collection)
            {
                if (item.result is not null &&
                    item.result.Count() > 0)
                {
                    res.Add(item);
                }
            }

            return res;
        }
        public async Task<List<EthTrainData>> GetTokensToProcess()
        {
            var res = await
                dbContext.
                EthTrainData.
                Where(
                    x =>
                    x.pairAddress != "no" &&
                    x.DeadBlockNumber > lastBlockToProcess &&
                    x.blockNumberInt < lastBlockToProcess
                    ).
                OrderBy(x => x.blockNumberInt).
                Where(x => x.pairAddress == "0xb1b665fb26d29934a678e79de4d95edb0bf2c33e").
                ToListAsync();

            return res;
        }

        public async Task<List<getSwapDTO>> Get(List<EthTrainData> ethTrainDatas)
        {
            List<getSwapDTO> res = new();

            var diff = ethTrainDatas.Count();
            var items = ethTrainDatas;

            List<(string, string, string)> t = new();

            foreach (var item in ethTrainDatas)
            {
                t.Add((item.pairAddress, "0x" + lastProcessedBlock.ToString("x"), "0x" + lastBlockToProcess.ToString("x")));
            }

            Func<List<(string, string, string)>, int, Task<List<getSwapDTO>>> apiMethod = apiAlchemy.getSwapLogs;

            res = await apiAlchemy.executeBatchCall(t, apiMethod, diff);

            return res;
        }
    }
}
