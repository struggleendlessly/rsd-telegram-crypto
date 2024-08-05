using api_alchemy.Eth;

using Data;

using Microsoft.Extensions.Logging;

namespace eth_shared
{
    public class IsDead
    {
        private readonly ILogger logger;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;
        public IsDead(
            ILogger<IsDead> logger,
            EthApi apiAlchemy,
            dbContext dbContext
            )
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;
        }

        public async Task Start()
        {
            //var lastEthBlockNumber = await apiAlchemy.lastBlockNumber();
            //var lastProcessedBlock = await dbContext.Blocks.MaxAsync(x => x.BlockNumber);

            //var unfiltered = await Get(lastEthBlockNumber, lastProcessedBlock);
            //var validated = Validate(unfiltered);
            //var res = Filter(validated);

            //await SaveToDB(res);
        }
    }
}
