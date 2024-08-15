using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using nethereum;

using System.Globalization;

namespace eth_shared
{
    public class GetReservesLogs
    {
        private readonly ILogger logger;
        private readonly ApiWeb3 ApiWeb3;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;

        int lastEthBlockNumber = 0;
        int lastProcessedBlock = 18911035;
        int lastBlockToProcess = 0;

        CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        string decimalCeparator = ".";

        public GetReservesLogs(
            ILogger<GetReservesLogs> logger,
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
            //var validated = Validate(unfiltered);

            //if (validated.Count == 0)
            //{
            //    var t = new EthSwapEvents();
            //    t.blockNumberInt = lastBlockToProcess;

            //    dbContext.EthSwapEvents.Add(t);
            //    await dbContext.SaveChangesAsync();
            //}
            //else
            //{
            //    var decoded = DecodeSwapEvents(validated);
            //    var processed = ProcessDecoded(decoded, tokensToProcess);
            //    var saved = await SaveToDB_update(processed);
            //}
        }

        public async Task<List<getSwapDTO>> Get(List<EthTrainData> ethTrainDatas)
        {
            List<getSwapDTO> res = new();

            var diff = ethTrainDatas.Count();
            var items = ethTrainDatas;

            List<(string, string, string)> t = new();

            t.Add(("0xc45a81bc23a64ea556ab4cdf08a86b61cdceea8b", "0x" + 20534360.ToString("x"), "0x" + 20534366.ToString("x")));


            Func<List<(string, string, string)>, int, Task<List<getSwapDTO>>> apiMethod = apiAlchemy.getReservesLogs;

            res = await apiAlchemy.executeBatchCall(t, apiMethod, diff);

            return res;
        }

        public async Task<List<EthTrainData>> GetTokensToProcess()
        {
            var res = await
                dbContext.
                EthTrainData.
                //Where(
                //    x =>
                //    x.pairAddress != "no" &&
                //    x.DeadBlockNumber > lastBlockToProcess &&
                //    x.blockNumberInt < lastBlockToProcess
                //    ).
                OrderBy(x => x.blockNumberInt).
                Take(100).
                //Where(x => x.pairAddress == "0xb1b665fb26d29934a678e79de4d95edb0bf2c33e").
                ToListAsync();

            return res;
        }
    }
}
