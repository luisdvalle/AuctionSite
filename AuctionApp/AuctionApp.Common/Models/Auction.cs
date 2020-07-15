using System;
using System.Collections.Generic;

namespace AuctionApp.Common.Models
{
    public class Auction
    {
        public string Status { get; set; }
        public DateTime StartingAt { get; set; }
        public DateTime FinishingAt { get; set; }
        public double SoldAt { get; set; }
        public Item Item { get; set; }
        public string OwnerEmail { get; set; }
        public string BuyerEmail { get; set; }
        public IEnumerable<string> BiddersEmails { get; set; }
    }
}
