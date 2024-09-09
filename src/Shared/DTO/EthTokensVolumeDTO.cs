using Nethereum.Util;

namespace Shared.DTO
{
    public class EthTokensVolumeDTO
    {
        public int Id { get; set; }

        public DateTime blockIntStartDate { get; set; }
        public int blockIntStart { get; set; }
        public DateTime blockIntEndDate { get; set; }
        public int blockIntEnd { get; set; }
        public bool isTlgrmMessageSent { get; set; }
        public int periodInMins { get; set; }

        public BigDecimal volumePositiveEth { get; set; }
        public BigDecimal volumeNegativeEth { get; set; }
        public BigDecimal volumeTotalEth { get; set; }

        public int EthTrainDataId { get; set; }
    }

    public class EthTokensVolumeAvarageDTO
    {
        public EthTokensVolumeDTO last = new();
        public int periodInMins { get; set; }

        public BigDecimal volumePositiveEthAverage = 0.0;
        public BigDecimal volumeNegativeEthAverage = 0.0;
        public BigDecimal volumeTotalEthAverage = 0.0;
        public int EthTrainDataId { get; set; }
    }
}
