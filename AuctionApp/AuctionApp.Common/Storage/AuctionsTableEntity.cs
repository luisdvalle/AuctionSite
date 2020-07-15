using System;
using System.Collections.Generic;
using AuctionApp.Common.Constants;
using AuctionApp.Common.Models;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AuctionApp.Common.Storage
{
    public class AuctionsTableEntity : TableEntity
    {
        public string Id => RowKey;
        public string Status { get; set; }
        public DateTime StartingAt { get; set; }
        public DateTime FinishingAt { get; set; }
        public double SoldAt { get; set; }
        public string OwnerEmail { get; set; }
        public string BuyerEmail { get; set; }
        [IgnoreProperty]
        public Item Item { get; set; }
        public string AuctionItem
        {
            get => JsonConvert.SerializeObject(Item);
            set => Item = JsonConvert.DeserializeObject<Item>(value);
        }
        [IgnoreProperty]
        public IDictionary<string, double> BiddersCollection { get; set; }
        public string Bidders
        {
            get => JsonConvert.SerializeObject(BiddersCollection);
            set => BiddersCollection = JsonConvert.DeserializeObject<IDictionary<string, double>>(value);
        }

        public AuctionsTableEntity(string ownerEmail, Item item, DateTime startingAt, DateTime finishingAt)
            : base(AuctionAppConstants.AuctionsTablePartitionKey,
                item.Name + "-" + ownerEmail)
        {
            Status = AuctionAppConstants.AuctionStatusInProgress;
            BiddersCollection = new Dictionary<string, double>();
            StartingAt = startingAt;
            FinishingAt = finishingAt;
            SoldAt = 0;
            OwnerEmail = ownerEmail;
            Item = item;
        }

        public AuctionsTableEntity()
        {
        }
    }
}
