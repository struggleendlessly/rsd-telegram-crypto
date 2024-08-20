using api_alchemy.Eth;

using Data;
using Data.Models;

using eth_shared.DTO;
using eth_shared.Map;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using nethereum;

using Nethereum.Util;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eth_shared
{
    public class VolumePrepare
    {
        private readonly ILogger logger;
        private readonly dbContext dbContext;
        private readonly EthApi apiAlchemy;

        int lastEthBlockNumber = 0;
        int lastProcessedBlock = 18911035;
        int lastBlockToProcess = 0;
        int periodInMins = 5;
        public VolumePrepare(
            ILogger<VolumePrepare> logger,
            dbContext dbContext,
            EthApi apiAlchemy
            )
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;
        }

        public async Task Start(
            int periodInMins = 5
            )
        {
            lastEthBlockNumber = await apiAlchemy.lastBlockNumber();
            this.periodInMins = periodInMins;

            if (await dbContext.EthTokensVolumes.AnyAsync())
            {
                lastProcessedBlock = await dbContext.EthTokensVolumes.MaxAsync(x => x.blockIntEnd) + 1;
            }

            lastBlockToProcess = lastProcessedBlock + periodInMins * 5;

            if (lastBlockToProcess > lastEthBlockNumber)
            {
                return;
            }

            var tokensToProcess = await GetTokensToProcess();
            List<EthSwapEventsDTO> mapped = new();

            foreach (var item in tokensToProcess)
            {
                var t = item.Map();
                mapped.Add(t);
            }

            var grouped = mapped.GroupBy(x => x.pairAddress).ToList();

            List<EthTokensVolume> result = new();

            foreach (var group in grouped)
            {
                EthTokensVolume tVolume = new();
                tVolume.blockIntStart = lastProcessedBlock;
                tVolume.blockIntEnd = lastBlockToProcess;
                tVolume.EthTrainData = group.First().EthTrainData;
                tVolume.periodInMins = periodInMins;

                BigDecimal volumePositiveEth = 0;
                BigDecimal volumeNegativeEth = 0;
                BigDecimal volumeTotalEth = 0;

                foreach (var item in group)
                {
                    if (item.isBuy)
                    {
                        volumePositiveEth += item.EthIn;
                    }
                    else
                    {
                        volumeNegativeEth += item.EthOut;
                    }
                }

                volumeTotalEth = (volumePositiveEth + volumeNegativeEth);

                tVolume.volumePositiveEth = volumePositiveEth.ToString();
                tVolume.volumeNegativeEth = volumeNegativeEth.ToString();
                tVolume.volumeTotalEth = volumeTotalEth.ToString();

                result.Add(tVolume);
            }

            if (result.Count == 0)
            {
                EthTokensVolume tVolume = new();
                tVolume.blockIntStart = lastProcessedBlock;
                tVolume.blockIntEnd = lastBlockToProcess;
                result.Add(tVolume);
            }

            dbContext.EthTokensVolumes.AddRange(result);
            await dbContext.SaveChangesAsync();

        }

        public async Task<List<EthSwapEvents>> GetTokensToProcess()
        {
            var res = await
                dbContext.
                EthSwapEvents.
                Where(x => x.blockNumberInt >= lastProcessedBlock && x.blockNumberInt < lastBlockToProcess).
                ToListAsync();

            return res;
        }
    }
}
