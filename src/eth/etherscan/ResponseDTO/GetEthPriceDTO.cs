namespace etherscan.ResponseDTO
{
    public class GetEthPriceDTO
    {
        public string status { get; set; }
        public string message { get; set; }
        public Result result { get; set; }
    }

    public class Result
    {
        public string ethbtc { get; set; }
        public string ethbtc_timestamp { get; set; }
        public string ethusd { get; set; }
        public string ethusd_timestamp { get; set; }
    }

}
