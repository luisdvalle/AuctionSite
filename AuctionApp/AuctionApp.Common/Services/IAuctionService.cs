using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionApp.Common.Storage;

namespace AuctionApp.Common.Services
{
    public interface IAuctionService
    {
        Task ProcessAuctions();
    }
}
