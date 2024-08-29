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
    public class VolumeTracking
    {
        private readonly ILogger logger;
        private readonly dbContext dbContext;
        private readonly EthApi apiAlchemy;
        private readonly tlgrmApi.tlgrmApi tlgrmApi;

        int lastEthBlockNumber = 0;
        int lastProcessedBlock = 18911035;
        int lastBlockToProcess = 0;
        int lastBlockInDbEthTokensVolumes = 0;
        int lastBlockInDbEthSwapEvents = 0;
        int periodInMins = 5;
        public VolumeTracking(
            ILogger<VolumeTracking> logger,
            dbContext dbContext,
            tlgrmApi.tlgrmApi tlgrmApi,
            EthApi apiAlchemy
            )
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;
            this.tlgrmApi = tlgrmApi;
        }

        public async Task Start(
            int periodInMins = 5
            )
        {
            lastEthBlockNumber = await apiAlchemy.lastBlockNumber();
            this.periodInMins = periodInMins;

            var tokensToProcess = await GetTokensToProcess();
            var mapped = Map(tokensToProcess);
            var average = Average(mapped);
            var validated = Validate(average);
            await SendTlgrmMessageP0(validated);
        }
        async Task SendTlgrmMessageP0(
            List<EthTokensVolumeAvarageDTO> validated)
        {
            var ethTrainData =
                dbContext.
                EthTrainData.
                Where(x => validated.Select(v => v.EthTrainDataId).Contains(x.Id)).
                ToList();

            var ids = ethTrainData.Select(x => x.blockNumberInt).ToList();
            var blocks = dbContext.EthBlock.Where(x => ids.Contains(x.numberInt)).ToList();

            var t = await tlgrmApi.SendP20(ethTrainData, blocks, validated);

            foreach (var item in ethTrainData)
            {
                var resp = t.FirstOrDefault(x => x.contractAddress.Equals(item.contractAddress, StringComparison.InvariantCultureIgnoreCase));

                if (resp is not null)
                {
                    item.tlgrmVolume = resp.tlgrmMsgId;
                }
            }

            //await dbContext.SaveChangesAsync();
        }

        private List<EthTokensVolumeAvarageDTO> Validate(
            List<EthTokensVolumeAvarageDTO> average)
        {
            List<EthTokensVolumeAvarageDTO> res = [];

            foreach (var item in average)
            {
                if (item.last.volumePositiveEth > (item.volumePositiveEthAverage * 3))
                {
                    res.Add(item);
                }

                if (item.last.Id == 1646)
                {
                    res.Add(item);
                }
            }

            return res;
        }

        private List<EthTokensVolumeAvarageDTO> Average(
            List<List<EthTokensVolumeDTO>> mapped)
        {
            List<EthTokensVolumeAvarageDTO> res = [];

            foreach (var groups in mapped)
            {
                if (groups.Count < 10)
                {
                    continue;
                }

                EthTokensVolumeAvarageDTO t = new();

                BigDecimal volumePositiveEthSum = 0.0;
                BigDecimal volumeNegativeEthSum = 0.0;
                BigDecimal volumeTotalEthSum = 0.0;

                var count = groups.Count() - 1;

                foreach (var item in groups)
                {
                    if (groups[groups.Count - 1] == item)
                    {
                        t.last = item;

                        continue;
                    }

                    volumePositiveEthSum += item.volumePositiveEth;
                    volumeNegativeEthSum += item.volumeNegativeEth;
                    volumeTotalEthSum += item.volumeTotalEth;
                }

                t.volumePositiveEthAverage = volumePositiveEthSum / count;
                t.volumeNegativeEthAverage = volumeNegativeEthSum / count;
                t.volumeTotalEthAverage = volumeTotalEthSum / count;
                t.EthTrainDataId = groups[0].EthTrainDataId;
                t.periodInMins = groups[0].periodInMins;

                res.Add(t);
            }

            return res;
        }
        private List<List<EthTokensVolumeDTO>> Map(
            List<IEnumerable<EthTokensVolume>> tokensToProcess)
        {
            List<List<EthTokensVolumeDTO>> res = [];

            foreach (var groups in tokensToProcess)
            {
                List<EthTokensVolumeDTO> mappedGroup = [];

                foreach (var item in groups)
                {
                    var t = item.Map();
                    mappedGroup.Add(t);
                }

                res.Add(mappedGroup);
            }

            return res;
        }
        public async Task<List<IEnumerable<EthTokensVolume>>> GetTokensToProcess()
        {
            List<IEnumerable<EthTokensVolume>> res = [];

            var ww = dbContext.EthTrainData.Where(x => x.isDead == false).Select(x => x.Id).ToList();
            res = await dbContext.
                EthTokensVolumes.
                Where(x => x.EthTrainDataId != null && ww.Contains((int)x.EthTrainDataId) &&
                      x.periodInMins == periodInMins).
                OrderByDescending(x => x.Id).
                GroupBy(x => x.EthTrainDataId).
                Select(g => g.OrderBy(row => row.Id).Take(21)).
                ToListAsync();

            return res;
        }
    }
}
