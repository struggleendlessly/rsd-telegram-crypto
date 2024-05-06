using Microsoft.IdentityModel.Tokens;

using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    /// <summary>
    /// Check if the name of the token is valid (not empty or does not contain .)
    /// </summary>
    public class CheckTheNameOfTokenHandler : AbstractHandler
    {
        public async override Task<AddressRequest> Handle(AddressRequest request)
        {
            if (request.IsValid)
            {
                request.IsValid = await IsValid(request);
            }

            return await base.Handle(request);

        }
        private async Task<bool> IsValid(AddressRequest request)
        {
            var res = false;

            //var message = request.TokenInfo.TelegramMessage;
            //var splited = message.Split(Environment.NewLine.ToCharArray())[2];//.Replace("Token: ");

            //var tokenName = splited.Replace("Token: ", "").Replace("👾", "").Trim();

            //if (!tokenName.Contains(".") &&
            //    !tokenName.Contains("test", StringComparison.InvariantCultureIgnoreCase) &&
            //    !tokenName.Contains("()"))
            //{
            //    res = true;
            //}

            return res;
        }
    }
}
