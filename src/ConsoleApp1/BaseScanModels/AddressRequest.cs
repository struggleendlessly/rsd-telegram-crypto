namespace ConsoleApp1.BaseScanModels
{
    public class AddressRequest
    {
        public AddressModel AddressModel { get; set; }
        public TokenInfo TokenInfo { get; set; }
        public bool IsValid { get; set; } = true;
    }
}
