using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AuctionApp.Common.Models;

namespace AuctionApp.Models
{
    public class AuctionBidViewModel
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public DateTime StartingAt { get; set; }
        public DateTime FinishingAt { get; set; }
        public string OwnerEmail { get; set; }
        public string ItemName { get; set; }
        [DataType(DataType.Currency)]
        [DisplayName("Your current Bid")]
        public double BuyersBid { get; set; }
    }
}
