using AuctionApp.Common.Constants;
using Microsoft.WindowsAzure.Storage.Table;

namespace AuctionApp.Common.Storage
{
    public class ItemsTableEntity : TableEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }

        public ItemsTableEntity(string name, string userEmail, double price)
            : base(AuctionAppConstants.ItemsTablePartitionKey, name + "-" + userEmail)
        {
            Name = name;
            Email = userEmail;
            Price = price;
            Status = AuctionAppConstants.ItemStatusAwaiting;
        }

        public ItemsTableEntity()
        {
        }
    }
}
