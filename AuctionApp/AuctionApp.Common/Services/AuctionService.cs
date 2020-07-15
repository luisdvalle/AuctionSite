using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionApp.Common.Constants;
using AuctionApp.Common.Storage;

namespace AuctionApp.Common.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IRepository _repository;

        public AuctionService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AuctionsTableEntity>> ProcessAuctions()
        {
            var auctionsTableEntities =
                await _repository.GetAllEntitiesAsync<AuctionsTableEntity>(StorageTablesNames.Auctions,
                    AuctionAppConstants.AuctionsTablePartitionKey);
            var entitiesProcessed = new List<AuctionsTableEntity>();

            foreach (var auctionsTableEntity in auctionsTableEntities)
            {
                if (DateTime.UtcNow < auctionsTableEntity.FinishingAt ||
                    auctionsTableEntity.Status != AuctionAppConstants.AuctionStatusInProgress)
                {
                    continue;
                }

                var bestBidder = auctionsTableEntity.BiddersCollection.Aggregate((currentBidder, nextBidder) =>
                    currentBidder.Value > nextBidder.Value ? currentBidder : nextBidder);
                var auctionItem = await _repository.GetEntityAsync<ItemsTableEntity>(StorageTablesNames.Items,
                    auctionsTableEntity.Id, AuctionAppConstants.ItemsTablePartitionKey);

                auctionItem.Status = bestBidder.Value < auctionItem.Price
                    ? AuctionAppConstants.ItemStatusAwaiting
                    : AuctionAppConstants.ItemStatusSold;
                await _repository.InsertOrReplaceEntityAsync(StorageTablesNames.Items, auctionItem);

                if (auctionsTableEntity.BiddersCollection.Count == 0 || bestBidder.Value <= 0)
                {
                    auctionsTableEntity.Status = AuctionAppConstants.AuctionStatusFinished;
                    await _repository.InsertOrReplaceEntityAsync(StorageTablesNames.Auctions, auctionsTableEntity);
                    entitiesProcessed.Add(auctionsTableEntity);

                    continue;
                }

                auctionsTableEntity.SoldAt = bestBidder.Value;
                auctionsTableEntity.BuyerEmail = bestBidder.Key;
                auctionsTableEntity.Status = AuctionAppConstants.AuctionStatusFinished;
                await _repository.InsertOrReplaceEntityAsync(StorageTablesNames.Auctions, auctionsTableEntity);
                entitiesProcessed.Add(auctionsTableEntity);
            }

            return entitiesProcessed;
        }
    }
}
