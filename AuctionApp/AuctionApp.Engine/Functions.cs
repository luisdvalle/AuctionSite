using System.Linq;
using System.Threading.Tasks;
using AuctionApp.Common.Services;
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
            ILogger logger)
        {
            
            var auctionsProcessed = await _auctionService.ProcessAuctions();

            logger.LogInformation($"Number of Auctions processed: {auctionsProcessed.Count()}");
        }
    }
}
