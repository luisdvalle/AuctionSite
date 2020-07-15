using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuctionApp.Common.Models
{
    public class Item
    {
        [DisplayName("Item Name")]
        public string Name { get; set; }
        [DataType(DataType.Currency)]
        public double Price { get; set; }
        public string Status { get; set; }
    }
}
