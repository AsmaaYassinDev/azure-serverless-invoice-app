using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
namespace InvoiceBackend
{
    public class InvoiceModel
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string CustomerName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // This will now automatically calculate the total every time!
        public double Amount => Items.Sum(item => item.Price * item.Quantity);

        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }

    public class InvoiceItem
    {
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}