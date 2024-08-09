using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using eth_shared.Extensions;
using eth_shared.Map;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using nethereum;

using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Nethereum.Util;

using System.Globalization;

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

        CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        string decimalCeparator = ".";

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
                await dbContext.SaveChangesAsync();
            }
            else
            {
                var decoded = DecodeSwapEvents(validated);
                var processed = ProcessDecoded(decoded, tokensToProcess);
                var saved = await SaveToDB_update(processed);
            }
        }

        private async Task<int> SaveToDB_update
            (List<EthSwapEvents> ethSwapEvents)
        {
            var res = 0;

            dbContext.EthSwapEvents.AddRange(ethSwapEvents);
            res = await dbContext.SaveChangesAsync();

            return res;
        }

        public List<EthSwapEvents> ProcessDecoded(
            List<EventLog<List<ParameterOutput>>> decoded,
            List<EthTrainData> ethTrainDatas
            )
        {
            List<EthSwapEvents> res = new();

            foreach (var item in decoded)
            {
                var logs = item.Log;
                var events = item.Event;
                var ethTrainData = ethTrainDatas.Where(x => x.pairAddress.Equals(logs.Address, StringComparison.InvariantCultureIgnoreCase)).Single();

                var ethSwapEvents = events.Map(ethTrainData, decimalCeparator);

                ethSwapEvents.pairAddress = logs.Address;
                ethSwapEvents.blockNumberInt = Convert.ToInt32(logs.BlockNumber.ToString());

                BigDecimal EthIn = BigDecimal.Parse(ethSwapEvents.EthIn);
                BigDecimal EthOut = BigDecimal.Parse(ethSwapEvents.EthOut);
                BigDecimal TokenIn = BigDecimal.Parse(ethSwapEvents.TokenIn);
                BigDecimal TokenOut = BigDecimal.Parse(ethSwapEvents.TokenOut);

                BigDecimal price = 0.0;

                if (EthIn > 0 &&
                    TokenOut > 0)
                {
                    // token0 is being bought with token1
                    price = EthIn / TokenOut;
                    ethSwapEvents.isBuy = true;
                }

                if (EthOut > 0 &&
                    TokenIn > 0)

                {
                    // token0 is being sold for token1
                    price = EthOut / TokenIn;
                }

                ethSwapEvents.txsHash = logs.TransactionHash;
                ethSwapEvents.priceEth = (double)price;
                ethSwapEvents.EthTrainData = ethTrainData;

                res.Add(ethSwapEvents);
            }

            return res;
        }

        public List<EventLog<List<ParameterOutput>>> DecodeSwapEvents(List<getSwapDTO> validated)
        {
            var logs = validated.SelectMany(x => x.result);
            var logsJson = System.Text.Json.JsonSerializer.Serialize(logs);
            var res = ApiWeb3.DecodeSwapEvents(logsJson);

            return res;
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
                //Where(x => x.pairAddress == "0xb1b665fb26d29934a678e79de4d95edb0bf2c33e").
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
