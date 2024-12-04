namespace dbMigration.models
{
    public class EthBlocksEntity
    {
        public int Id { get; set; }

        // Block
        public string numberHex { get; set; } = string.Empty;
        public int numberInt { get; set; } = 0;

        public string timestampUnix { get; set; } = string.Empty;
        public DateTime timestampNormal { get; set; } = new();

        public string baseFeePerGas { get; set; } = string.Empty;
        public string gasLimit { get; set; } = string.Empty;
        public string gasUsed { get; set; } = string.Empty;

        public static EthBlocksEntity Default()
        {
            var res = new EthBlocksEntity();
            res.numberInt = 21329788;

            return res;
        }
    }
}