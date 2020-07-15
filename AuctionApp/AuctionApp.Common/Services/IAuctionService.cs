using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionApp.Common.Storage;

namespace AuctionApp.Common.Services
{
    /// <summary>
    /// Defines functionality to process Auctions
    /// </summary>
    public interface IAuctionService
    {
        /// <summary>
        /// Processes all active Auctions in order to determine the winner bid
        /// </summary>
        /// <returns>An enumerable with all Auctions processed</returns>
        Task<IEnumerable<AuctionsTableEntity>> ProcessAuctions();
    }
}
