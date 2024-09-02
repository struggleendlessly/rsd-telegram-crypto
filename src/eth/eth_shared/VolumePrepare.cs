using api_alchemy.Eth;

using Data;
using Data.Models;

using eth_shared.Map;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Nethereum.Util;

using Shared.DTO;

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
        int multiply = 100;
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
            int periodInMins = 5,
            int multiply = 100
            )
        {
            lastEthBlockNumber = await apiAlchemy.lastBlockNumber();
            this.periodInMins = periodInMins;
            this.multiply = multiply;

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

            lastBlockToProcess = lastProcessedBlock + periodInMins * 5 * multiply;

            if (lastBlockToProcess > lastEthBlockNumber)
            {
                if ((lastProcessedBlock + periodInMins * 5) > lastEthBlockNumber)
                {
                    return;
                }
                else
                {
                    lastBlockToProcess = lastEthBlockNumber;
                }
            }

            if (lastBlockToProcess > lastBlockInDbEthSwapEvents)
            {
                if ((lastProcessedBlock + periodInMins * 5) > lastBlockInDbEthSwapEvents)
                {
                    return;
                }
                else
                {
                    lastBlockToProcess = lastBlockInDbEthSwapEvents;
                }
            }

            var tokensToProcess = await GetTokensToProcess();

            List<EthSwapEventsDTO> mapped = new();

            foreach (var item in tokensToProcess)
            {
                var t = item.Map();
                mapped.Add(t);
            }

            var grouped =
                mapped.
                GroupBy(x => x.pairAddress).
                ToList();

            List<EthTokensVolume> resultTemp = new();
            List<EthTokensVolume> result = new();

            foreach (var group in grouped)
            {
                var fromBlock = lastProcessedBlock;

                for (int i = lastProcessedBlock + periodInMins * 5; i < lastBlockToProcess; i = i + periodInMins * 5)
                {
                    EthTokensVolume tVolume = new();
                    var batch = group.Where(x => x.blockNumberInt >= fromBlock && x.blockNumberInt < i).ToList();

                    tVolume.blockIntStart = fromBlock;
                    tVolume.blockIntEnd = i;
                    tVolume.periodInMins = periodInMins;

                    if (batch.Count == 0)
                    {
                        tVolume.volumePositiveEth = 0.ToString();
                        tVolume.volumeNegativeEth = 0.ToString();
                        tVolume.volumeTotalEth = 0.ToString();
                        tVolume.EthTrainDataId = group.ElementAt(0).EthTrainDataId;
                    }
                    else
                    {
                        var swaped = tokensToProcess.Where(x => x.pairAddress == group.Key).FirstOrDefault();


                        BigDecimal volumePositiveEth = 0;
                        BigDecimal volumeNegativeEth = 0;
                        BigDecimal volumeTotalEth = 0;

                        foreach (var item in batch)
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
                        tVolume.EthTrainDataId = batch[0].EthTrainDataId;

                    }

                    resultTemp.Add(tVolume);

                    fromBlock = i;
                }
            }

            result = resultTemp;

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
                         x.EthTrainDataId != null).
                   ToListAsync();
            }

            logger.LogInformation("Worker Worker4Scoped volumePrepare GetTokensToProcess count: {count}, period: {period}", res.Count(), periodInMins);

            return res;
        }
    }
}
