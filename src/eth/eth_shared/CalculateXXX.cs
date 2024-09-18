using api_alchemy.Eth;
using Data;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eth_shared
{
    public class CalculateXXX
    {
        private readonly ILogger logger;
        private readonly dbContext dbContext;

        public CalculateXXX(
            ILogger<CalculateXXX> logger,
            dbContext dbContext
            )
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task Start()
        {
            logger.LogInformation("CalculateXXX Start");
        }
    }
}
