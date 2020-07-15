namespace AuctionApp.Common.Constants
{
    public class AuctionAppConstants
    {
        public const string AuctionsTablePartitionKey = "auction";
        public const string ItemsTablePartitionKey = "item";

        public const string ItemStatusOnSale = "on sale";
        public const string ItemStatusSold = "sold";
        public const string ItemStatusAwaiting = "awaiting auction";

        public const string AuctionStatusInProgress = "in progress";
        public const string AuctionStatusFinished = "finished";
    }
}
