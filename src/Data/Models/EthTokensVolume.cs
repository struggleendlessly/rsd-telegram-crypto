using Microsoft.EntityFrameworkCore;

namespace Data.Models
{
    [Index(nameof(blockIntEnd))]
    [Index(nameof(periodInMins))]
    public class EthTokensVolume
    {
        public int Id { get; set; }

        public int blockIntStart { get; set; }
        public int blockIntEnd { get; set; }
        public bool isTlgrmMessageSent { get; set; }

        public int periodInMins { get; set; }

        public string volumePositiveEth { get; set; } = string.Empty;
        public string volumeNegativeEth { get; set; } = string.Empty;
        public string volumeTotalEth { get; set; } = string.Empty;

        public int? EthTrainDataId { get; set; } = null;
        public EthTrainData? EthTrainData { get; set; } = null;
    }
}