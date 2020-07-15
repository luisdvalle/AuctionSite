using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionApp.Common.Constants;
using AuctionApp.Common.Services;
using AuctionApp.Common.Storage;
using Moq;
using Xunit;

namespace AuctionApp.Test.Services
{
    public class AuctionServiceTests
    {
        private IEnumerable<AuctionsTableEntity> _auctionsTestData1;
        private IEnumerable<AuctionsTableEntity> _auctionsTestData2;
        private IEnumerable<AuctionsTableEntity> _auctionsTestData3;
        private IEnumerable<AuctionsTableEntity> _auctionsTestData4;
        private IEnumerable<AuctionsTableEntity> _auctionsTestData5;
        private ItemsTableEntity _itemsTableEntity;

        public AuctionServiceTests()
        {
            GenerateTestData();
        }

        [Fact]
        public async Task ProcessAuctionsAsync_OneAuctionWithStatusInProgressThatHasNotReachedFinishedTime_ReturnsZeroAuctionsProcessed()
        {
            var fakeRepository = new Mock<IRepository>();
            fakeRepository.Setup(m => m.GetAllEntitiesAsync<AuctionsTableEntity>(StorageTablesNames.Auctions,
                    AuctionAppConstants.AuctionsTablePartitionKey))
                .ReturnsAsync(_auctionsTestData1);
            var fakeAuctionService = new AuctionService(fakeRepository.Object);

            var mockResult = await fakeAuctionService.ProcessAuctionsAsync();

            Assert.Empty(mockResult);
        }

        [Fact]
        public async Task ProcessAuctionsAsync_OneAuctionWithStatusFinished_ReturnsZeroAuctionsProcessed()
        {
            var fakeRepository = new Mock<IRepository>();
            fakeRepository.Setup(m => m.GetAllEntitiesAsync<AuctionsTableEntity>(StorageTablesNames.Auctions,
                    AuctionAppConstants.AuctionsTablePartitionKey))
                .ReturnsAsync(_auctionsTestData2);
            var fakeAuctionService = new AuctionService(fakeRepository.Object);

            var mockResult = await fakeAuctionService.ProcessAuctionsAsync();

            Assert.Empty(mockResult);
        }

        [Fact]
        public async Task ProcessAuctionsAsync_OneActiveAuctionThatHasReachedFinishingTimeAndWithNoBidders_ReturnsOneAuctionProcessedWithStatusFinished()
        {
            var fakeRepository = new Mock<IRepository>();
            fakeRepository.Setup(m => m.GetAllEntitiesAsync<AuctionsTableEntity>(StorageTablesNames.Auctions,
                    AuctionAppConstants.AuctionsTablePartitionKey))
                .ReturnsAsync(_auctionsTestData3);
            fakeRepository.Setup(m => m.GetEntityAsync<ItemsTableEntity>(StorageTablesNames.Items,
                    "fakeId", AuctionAppConstants.ItemsTablePartitionKey))
                .ReturnsAsync(_itemsTableEntity);
            var fakeAuctionService = new AuctionService(fakeRepository.Object);

            var mockResult = (await fakeAuctionService.ProcessAuctionsAsync()).ToList();

            Assert.Single(mockResult);
            Assert.Equal(AuctionAppConstants.AuctionStatusFinished, mockResult.First().Status);
        }

        [Fact]
        public async Task ProcessAuctionsAsync_OneActiveAuctionThatHasReachedFinishingTimeAndWithBestBidLessThanProductPrice_ReturnsOneAuctionProcessedWithStatusFinished()
        {
            var fakeRepository = new Mock<IRepository>();
            fakeRepository.Setup(m => m.GetAllEntitiesAsync<AuctionsTableEntity>(StorageTablesNames.Auctions,
                    AuctionAppConstants.AuctionsTablePartitionKey))
                .ReturnsAsync(_auctionsTestData4);
            fakeRepository.Setup(m => m.GetEntityAsync<ItemsTableEntity>(StorageTablesNames.Items,
                    "fakeId", AuctionAppConstants.ItemsTablePartitionKey))
                .ReturnsAsync(_itemsTableEntity);
            var fakeAuctionService = new AuctionService(fakeRepository.Object);

            var mockResult = (await fakeAuctionService.ProcessAuctionsAsync()).ToList();

            Assert.Single(mockResult);
            Assert.Equal(AuctionAppConstants.AuctionStatusFinished, mockResult.First().Status);
        }

        [Fact]
        public async Task ProcessAuctionsAsync_OneActiveAuctionThatHasReachedFinishingTimeAndWithBestBidGreaterThanProductPrice_ReturnsOneAuctionProcessedWithStatusFinishedAndSoldToBestBidder()
        {
            var fakeRepository = new Mock<IRepository>();
            fakeRepository.Setup(m => m.GetAllEntitiesAsync<AuctionsTableEntity>(StorageTablesNames.Auctions,
                    AuctionAppConstants.AuctionsTablePartitionKey))
                .ReturnsAsync(_auctionsTestData5);
            fakeRepository.Setup(m => m.GetEntityAsync<ItemsTableEntity>(StorageTablesNames.Items,
                    "fakeId", AuctionAppConstants.ItemsTablePartitionKey))
                .ReturnsAsync(_itemsTableEntity);
            var fakeAuctionService = new AuctionService(fakeRepository.Object);

            var mockResult = (await fakeAuctionService.ProcessAuctionsAsync()).ToList();

            Assert.Single(mockResult);
            Assert.Equal(AuctionAppConstants.AuctionStatusFinished, mockResult.First().Status);
            Assert.Equal("user@email.com", mockResult.First().BuyerEmail);
            Assert.Equal(60, mockResult.First().SoldAt);
        }

        private void GenerateTestData()
        {
            _auctionsTestData1 = new List<AuctionsTableEntity>
            {
                new AuctionsTableEntity
                {
                    FinishingAt = DateTime.UtcNow.AddMinutes(60),
                    Status = AuctionAppConstants.AuctionStatusInProgress
                }
            };

            _auctionsTestData2 = new List<AuctionsTableEntity>
            {
                new AuctionsTableEntity
                {
                    FinishingAt = DateTime.UtcNow,
                    Status = AuctionAppConstants.AuctionStatusFinished
                }
            };

            _auctionsTestData3 = new List<AuctionsTableEntity>
            {
                new AuctionsTableEntity
                {
                    RowKey = "fakeId",
                    FinishingAt = DateTime.UtcNow,
                    Status = AuctionAppConstants.AuctionStatusInProgress,
                    BiddersCollection = new Dictionary<string, double>()
                }
            };

            _auctionsTestData4 = new List<AuctionsTableEntity>
            {
                new AuctionsTableEntity
                {
                    RowKey = "fakeId",
                    FinishingAt = DateTime.UtcNow,
                    Status = AuctionAppConstants.AuctionStatusInProgress,
                    BiddersCollection = new Dictionary<string, double>
                    {
                        {"user", 40 }
                    }
                }
            };

            _auctionsTestData5 = new List<AuctionsTableEntity>
            {
                new AuctionsTableEntity
                {
                    RowKey = "fakeId",
                    FinishingAt = DateTime.UtcNow,
                    Status = AuctionAppConstants.AuctionStatusInProgress,
                    BiddersCollection = new Dictionary<string, double>
                    {
                        {"user@email.com", 60 }
                    }
                }
            };

            _itemsTableEntity = new ItemsTableEntity
            {
                RowKey = "fakeId",
                Name = "Car",
                Price = 50
            };
        }
    }
}
