using Data.Models;


using Nethereum.Util;

using Shared.DTO;

using System.Globalization;

namespace eth_shared.Map
{
    public static class EthTokensVolumeMapper
    {
        static CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        static string decimalCeparator = ".";
        public static EthTokensVolumeDTO Map(
             this EthTokensVolume val)
        {
            EthTokensVolumeDTO res = new();

            res.Id = val.Id;
            res.EthTrainDataId = (int)val.EthTrainDataId;
            res.blockIntEnd = val.blockIntEnd;
            res.blockIntStart = val.blockIntStart;
            res.periodInMins = val.periodInMins;
            res.volumeNegativeEth = BigDecimal.Parse(val.volumeNegativeEth);
            res.volumePositiveEth = BigDecimal.Parse(val.volumePositiveEth);
            res.volumeTotalEth = BigDecimal.Parse(val.volumeTotalEth);

            return res;
        }
    }
}
