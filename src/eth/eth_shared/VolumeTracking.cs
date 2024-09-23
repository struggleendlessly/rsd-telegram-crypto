using api_alchemy.Eth;

using Data;
using Data.Models;

using eth_shared.Map;

using etherscan;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using Nethereum.Util;

using Shared.DTO;

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace eth_shared
{
    public class VolumeTracking
    {
        private readonly ILogger logger;
        private readonly dbContext dbContext;
        private readonly EthApi apiAlchemy;
        private readonly tlgrmApi.tlgrmApi tlgrmApi;
        private readonly EtherscanApi etherscanApi;

        double ethPrice = 0.0;
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
            EthApi apiAlchemy,
            EtherscanApi etherscanApi
            )
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;
            this.tlgrmApi = tlgrmApi;
            this.etherscanApi = etherscanApi;
        }

        public async Task Start(
            int periodInMins = 5
            )
        {
            ethPrice = await etherscanApi.getEthPrice();

            lastEthBlockNumber = (await dbContext.EthBlock.OrderByDescending(x => x.numberInt).Take(1).SingleAsync()).numberInt;
            this.periodInMins = periodInMins;

            var tokensToProcess = await GetTokensToProcess();
            var mapped = Map(tokensToProcess);
            var average = Average(mapped);
            var validated = Validate(average);

            //TODO: double call to the DB with method SendTlgrmMessageP0 !!!!!!!!!!
            IEnumerable<int> EthTrainDataIds = validated.Select(v => v.EthTrainDataId);
            var ethTrainDatas =
                await
                dbContext.
                EthTrainData.
                Where(x => EthTrainDataIds.Contains(x.Id)).
                ToListAsync();

            await SendTlgrmMessageP0(validated);

            if (periodInMins == 5)
            {
                var validate03v100mc = await ValidateVandMC(
                    ethTrainDatas,
                    validated,
                    volumeLimit: 0.3,
                    marketCapLimitFrom: 0,
                    marketCapLimitTo: 100_000);

                await SendTlgrmMessageP0(validate03v100mc, "5mins_03v100mc");

                var validate09v01_1mc = await ValidateVandMC(
                    ethTrainDatas,
                    validated,
                    volumeLimit: 0.9,
                    marketCapLimitFrom: 100_000,
                    marketCapLimitTo: 1_000_000);

                await SendTlgrmMessageP0(validate09v01_1mc, "5mins_09v01_1mc");
            }

            if (periodInMins == 30)
            {
                var validate03v100mc = await ValidateVandMC(
                    ethTrainDatas,
                    validated,
                    volumeLimit: 0.3,
                    marketCapLimitFrom: 0,
                    marketCapLimitTo: 100_000);

                await SendTlgrmMessageP0(validate03v100mc, "30mins_03v100mc");

                var validate09v01_1mc = await ValidateVandMC(
                    ethTrainDatas,
                    validated,
                    volumeLimit: 0.9,
                    marketCapLimitFrom: 100_000,
                    marketCapLimitTo: 1_000_000);

                await SendTlgrmMessageP0(validate09v01_1mc, "30mins_09v01_1mc");
            }
        }

        private async Task<List<EthTokensVolumeAvarageDTO>> ValidateVandMC(
            List<EthTrainData> ethTrainDatas,
            List<EthTokensVolumeAvarageDTO> validated,
            double volumeLimit,
            int marketCapLimitFrom = 0,
            int marketCapLimitTo = 100_000)
        {
            List<EthTokensVolumeAvarageDTO> res = new();

            foreach (var item in validated)
            {
                var ethTrainData = ethTrainDatas.Where(x => x.Id == item.EthTrainDataId).FirstOrDefault();

                var totalSupply = BigDecimal.Parse(ethTrainData.totalSupply);

                var t1 =
                    dbContext.
                    EthSwapEvents.
                    Where(x => x.EthTrainDataId == item.EthTrainDataId).
                    OrderByDescending(x => x.Id).
                    Take(1).
                    FirstOrDefault();

                var marketCap = totalSupply * (BigDecimal)t1.priceEth * (BigDecimal)ethPrice;

                if (item.last.volumePositiveEth >= volumeLimit &&
                    marketCap > marketCapLimitFrom &&
                    marketCap <= marketCapLimitTo)
                {
                    res.Add(item);
                }
            }

            return res;
        }

        private List<EthTokensVolumeAvarageDTO> Validate02(List<EthTokensVolumeAvarageDTO> validated)
        {
            List<EthTokensVolumeAvarageDTO> res = new();

            foreach (var item in validated)
            {
                if (item.volumePositiveEthAverage >= 0.2)
                {
                    res.Add(item);
                }
            }

            return res;
        }

        async Task SendTlgrmMessageP0(
            List<EthTokensVolumeAvarageDTO> validated,
            string addition = "")
        {
            IEnumerable<int> EthTrainDataIds = validated.Select(v => v.EthTrainDataId);
            var ethTrainData =
                await
                dbContext.
                EthTrainData.
                Where(x => EthTrainDataIds.Contains(x.Id)).
                ToListAsync();

            var swaps =
                await
                dbContext.
                EthSwapEvents.
                Where(x => EthTrainDataIds.Contains((int)x.EthTrainDataId)).
                GroupBy(x => x.EthTrainDataId).
                Select(g => g.OrderByDescending(row => row.Id).Take(1)).
                ToListAsync();

            foreach (var item in ethTrainData)
            {
                var t1 =
                    swaps.
                    Where(x => x.Any(v => v.EthTrainDataId == item.Id)).
                    Select(x => x).
                    FirstOrDefault();

                if (t1 is not null)
                {
                    item.EthSwapEvents = new List<EthSwapEvents>(t1);
                }
            }


            var ids = ethTrainData.Select(x => x.blockNumberInt).ToList();
            var blocks = dbContext.EthBlock.Where(x => ids.Contains(x.numberInt)).ToList();
            var volumeRiseCount =
                dbContext.
                EthTokensVolumes.
                Where(x => EthTrainDataIds.Contains((int)x.EthTrainDataId) && x.isTlgrmMessageSent == true && x.periodInMins == periodInMins).
                ToList();

            var t = await
                tlgrmApi.
                SendP20(
                    ethTrainData,
                    blocks,
                    validated,
                    volumeRiseCount,
                    periodInMins,
                    addition);

            List<EthTokensVolume> EthTokensVolumesToUpdate = new();

            foreach (var item in ethTrainData)
            {
                var resp = t.FirstOrDefault(x => x.contractAddress.Equals(item.contractAddress, StringComparison.InvariantCultureIgnoreCase));

                if (resp is not null)
                {
                    item.tlgrmVolume = resp.tlgrmMsgId;
                }

                var tempVolumeValidated = validated.Where(x => x.EthTrainDataId == item.Id).FirstOrDefault();
                var tempVolumeUpdate = dbContext.EthTokensVolumes.Where(x => x.Id == tempVolumeValidated.last.Id).FirstOrDefault();
                tempVolumeUpdate.isTlgrmMessageSent = true;
                EthTokensVolumesToUpdate.Add(tempVolumeUpdate);
            }

            await dbContext.SaveChangesAsync();
        }

        private List<EthTokensVolumeAvarageDTO> Validate(
            List<EthTokensVolumeAvarageDTO> average)
        {
            List<EthTokensVolumeAvarageDTO> res = [];

            foreach (var item in average)
            {
                if (item.last.isTlgrmMessageSent)
                {
                    continue;
                }

                if (item.last.volumePositiveEth > (item.volumePositiveEthAverage * 3))
                {
                    var blockIntStart = dbContext.EthBlock.Where(x => x.numberInt == item.last.blockIntStart).FirstOrDefault();
                    var blockIntEnd = dbContext.EthBlock.Where(x => x.numberInt == item.last.blockIntEnd).FirstOrDefault();

                    if (blockIntStart is not null)
                    {
                        int intValue = Convert.ToInt32(blockIntStart.timestamp, 16);
                        var timestampInt = Convert.ToDouble(intValue);
                        item.last.blockIntStartDate = DateTime.UnixEpoch.AddSeconds(timestampInt);
                    }

                    if (blockIntEnd is not null)
                    {
                        int intValue = Convert.ToInt32(blockIntEnd.timestamp, 16);
                        var timestampInt = Convert.ToDouble(intValue);
                        item.last.blockIntEndDate = DateTime.UnixEpoch.AddSeconds(timestampInt);
                    }

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
                    if (groups[0] == item)
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
                GroupBy(x => x.EthTrainDataId).
                Select(g => g.OrderByDescending(row => row.blockIntEnd).Take(21)).
                ToListAsync();

            return res;
        }
    }
}
