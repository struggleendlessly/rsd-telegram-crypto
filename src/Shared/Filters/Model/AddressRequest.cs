using Shared.BaseScan.Model;
using Shared.DB;

namespace Shared.Filters.Model
{
    public class AddressRequest
    {
        public AddressModel AddressModel { get; set; }
        public DB.TokenInfo TokenInfo { get; set; }
        public bool IsValid { get; set; } = true;
        public bool isContractVerified { get; set; } = false;
    }
}
