using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionApp.Common.Constants;
using AuctionApp.Common.Services;
using AuctionApp.Common.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AuctionApp.Engine
{
    public class Functions
    {
        private readonly IAuctionService _auctionService;

        public Functions(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        public async Task ProcessTimeTrigger([TimerTrigger("0 */1 * * * *")] TimerInfo timer,
            //[Table("Auctions", AuctionAppConstants.AuctionsTablePartitionKey)] IQueryable auctionsTableEntities,
            //[Table("Auctions", AuctionAppConstants.AuctionsTablePartitionKey, "Fridge-jessica@testemail.com",
            //    Connection = "AzureWebJobsStorage")]
            //AuctionsTableEntity auctionsTableEntity,
            ILogger logger)
        {
            logger.LogInformation("");
            await _auctionService.ProcessAuctions();
        }
    }
}
