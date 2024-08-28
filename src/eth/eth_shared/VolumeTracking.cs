using api_alchemy.Eth;

using Data;
using Data.Models;

using eth_shared.DTO;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Nethereum.Util;

namespace eth_shared
{
    public class VolumeTracking
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
        public VolumeTracking(
            ILogger<VolumeTracking> logger,
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

            //var lastInDbEthTokensVolumes =
            //    await dbContext.
            //    EthTokensVolumes.
            //    Where(x => x.periodInMins == periodInMins).
            //    OrderByDescending(x => x.blockIntEnd).
            //    Take(1).
            //    FirstOrDefaultAsync();

            //if (lastInDbEthTokensVolumes is not null)
            //{
            //    lastBlockInDbEthTokensVolumes = lastInDbEthTokensVolumes.blockIntEnd;
            //}

            //lastBlockInDbEthSwapEvents =
            //    await dbContext.
            //    EthSwapEvents.
            //    MaxAsync(x => x.blockNumberInt);

            //if (await dbContext.EthTokensVolumes.AnyAsync(x => x.periodInMins == periodInMins))
            //{
            //    lastProcessedBlock = lastBlockInDbEthTokensVolumes + 1;
            //}

            //lastBlockToProcess = lastProcessedBlock + periodInMins * 5;

            //if (lastBlockToProcess > lastEthBlockNumber)
            //{
            //    return;
            //}

            //if (lastBlockToProcess > lastBlockInDbEthSwapEvents)
            //{
            //    return;
            //}

            var tokensToProcess = await GetTokensToProcess();

            List<EthSwapEventsDTO> mapped = new();

            //foreach (var item in tokensToProcess)
            //{
            //    var t = item.Map();
            //    mapped.Add(t);
            //}

            //var grouped = mapped.GroupBy(x => x.pairAddress).ToList();

            //List<EthTokensVolume> result = new();

            //foreach (var group in grouped)
            //{
            //    var swaped = tokensToProcess.Where(x => x.pairAddress == group.Key).FirstOrDefault();

            //    EthTokensVolume tVolume = new();
            //    tVolume.blockIntStart = lastProcessedBlock;
            //    tVolume.blockIntEnd = lastBlockToProcess;
            //    tVolume.EthTrainData = group.First().EthTrainData;
            //    tVolume.periodInMins = periodInMins;

            //    BigDecimal volumePositiveEth = 0;
            //    BigDecimal volumeNegativeEth = 0;
            //    BigDecimal volumeTotalEth = 0;

            //    foreach (var item in group)
            //    {
            //        if (item.isBuy)
            //        {
            //            volumePositiveEth += item.EthIn;
            //        }
            //        else
            //        {
            //            volumeNegativeEth += item.EthOut;
            //        }
            //    }

            //    volumeTotalEth = (volumePositiveEth + volumeNegativeEth);

            //    tVolume.volumePositiveEth = volumePositiveEth.ToString();
            //    tVolume.volumeNegativeEth = volumeNegativeEth.ToString();
            //    tVolume.volumeTotalEth = volumeTotalEth.ToString();
            //    tVolume.EthTrainData = swaped.EthTrainData;

            //    result.Add(tVolume);
            //}

            //if (result.Count == 0)
            //{
            //    EthTokensVolume tVolume = new();
            //    tVolume.blockIntStart = lastProcessedBlock;
            //    tVolume.blockIntEnd = lastBlockToProcess;
            //    tVolume.periodInMins = periodInMins;
            //    result.Add(tVolume);
            //}

            //dbContext.EthTokensVolumes.AddRange(result);
            //await dbContext.SaveChangesAsync();

        }

        public async Task<List<IGrouping<EthTrainData, EthTokensVolume>>> GetTokensToProcess()
        {
            List<IGrouping<EthTrainData, EthTokensVolume>> res = [];

            var res5 = await
                dbContext.
                EthTokensVolumes.
                Include(x => x.EthTrainData).
                Where(x => x.EthTrainData.isDead == false && x.periodInMins == periodInMins).
                GroupBy(x => x.EthTrainData).
                Select(g => g.OrderByDescending(row => row.Id).Take(20)).
                ToListAsync();

            return res;
        }
    }
}
