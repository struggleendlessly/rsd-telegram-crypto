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
        int lastBlockInDbEthTokensVolumes = 0;
        int lastBlockInDbEthSwapEvents = 0;
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

            var lastInDbEthTokensVolumes =
                await dbContext.
                EthTokensVolumes.
                Where(x => x.periodInMins == periodInMins).
                OrderByDescending(x => x.blockIntEnd).
                Take(1).
                FirstOrDefaultAsync();

            if (lastInDbEthTokensVolumes is not null)
            {
                lastBlockInDbEthTokensVolumes = lastInDbEthTokensVolumes.blockIntEnd;
            }

            lastBlockInDbEthSwapEvents =
                await dbContext.
                EthSwapEvents.
                MaxAsync(x => x.blockNumberInt);

            if (await dbContext.EthTokensVolumes.AnyAsync(x => x.periodInMins == periodInMins))
            {
                lastProcessedBlock = lastBlockInDbEthTokensVolumes + 1;
            }

            lastBlockToProcess = lastProcessedBlock + periodInMins * 5;

            if (lastBlockToProcess > lastEthBlockNumber)
            {
                return;
            }

            if (lastBlockToProcess > lastBlockInDbEthSwapEvents)
            {
                return;
            }

            var tokensToProcess = await GetTokensToProcess();

            List<EthSwapEventsDTO> mapped = new();

            foreach (var item in tokensToProcess)
            {
                try
                {
                    var t = item.Map();
                    mapped.Add(t);

                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            var grouped = mapped.GroupBy(x => x.pairAddress).ToList();

            List<EthTokensVolume> result = new();

            foreach (var group in grouped)
            {
                var swaped = tokensToProcess.Where(x => x.pairAddress == group.Key).FirstOrDefault();

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
                tVolume.EthTrainData = swaped.EthTrainData;

                result.Add(tVolume);
            }

            if (result.Count == 0)
            {
                EthTokensVolume tVolume = new();
                tVolume.blockIntStart = lastProcessedBlock;
                tVolume.blockIntEnd = lastBlockToProcess;
                tVolume.periodInMins = periodInMins;
                result.Add(tVolume);
            }

            dbContext.EthTokensVolumes.AddRange(result);
            await dbContext.SaveChangesAsync();

        }

        public async Task<List<EthSwapEvents>> GetTokensToProcess()
        {
            List<EthSwapEvents> res = [];

            if (periodInMins == 1)
            {
                var freshTokens = await
                    dbContext.
                    EthTrainData.
                    Where(x => (lastEthBlockNumber - x.blockNumberInt) <= 500).
                    Include(x => x.EthSwapEvents).
                    SelectMany(x => x.EthSwapEvents).
                    Where(x => x.blockNumberInt >= lastProcessedBlock &&
                          x.blockNumberInt < lastBlockToProcess &&
                          x.EthTrainData != null).
                    ToListAsync();
            }
            else
            {
                res = await
                    dbContext.
                    EthSwapEvents.
                    Where(x => x.blockNumberInt >= lastProcessedBlock &&
                          x.blockNumberInt < lastBlockToProcess &&
                          x.EthTrainData != null).
                    Include(x => x.EthTrainData).
                    ToListAsync();
            }


            return res;
        }
    }
}
