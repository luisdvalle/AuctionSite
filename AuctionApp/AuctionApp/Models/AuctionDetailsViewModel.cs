using System;
using System.Collections.Generic;

namespace AuctionApp.Models
{
    public class AuctionDetailsViewModel
    {
        public string Id { get; set; }
        public string ItemName { get; set; }
        public string Status { get; set; }
        public DateTime StartingAt { get; set; }
        public DateTime FinishingAt { get; set; }
        public double SoldAt { get; set; }
        public string BuyerEmail { get; set; }
        public IDictionary<string, double> Bidders { get; set; }
    }
}
