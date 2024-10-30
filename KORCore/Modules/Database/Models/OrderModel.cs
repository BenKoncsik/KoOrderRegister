using KORCore.Utility;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Modules.Database.Models
{
    [Table("Orders")]
    public class OrderModel
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public string Id { get; init; }
        [JsonProperty("start_order_date")]
        public DateTime StartDate { get; set; } = DateTime.Now;
        [JsonProperty("end_order_date")]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }
        [Ignore]
        [JsonProperty("customer")]
        public CustomerModel Customer { get; set; }
        [JsonProperty("product_id")]
        public string ProductId { get; set; }

        [JsonProperty("order_number")]
        public string OrderNumber { get; set; }

        [Ignore]
        [JsonProperty("files")]
        public List<FileModel> Files { get; set; } = new List<FileModel>();

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("price")]
        public float Price { get; set; }
        [Ignore]
        [JsonIgnore]
        public Guid Guid => Guid.Parse(Id);
        [Ignore]
        [JsonProperty("order_date")]
        public string OrderDate => $"{StartDate.ToString(DateFormat.DateTimeFormat)} - {EndDate.ToString(DateFormat.DateTimeFormat)}";

        public OrderModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        public OrderModel(string name) : this()
        {

        }
    }
}
