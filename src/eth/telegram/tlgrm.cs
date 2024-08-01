using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;

namespace telegram
{
    public class tlgrm
    {
        private readonly ILogger logger;
        private readonly OptionsTelegram optionsTelegram;

        public tlgrm(
            ILogger<tlgrm> logger,
            IOptions<OptionsTelegram> options
            )
        {
            this.optionsTelegram = options.Value;

        }
    }
}
