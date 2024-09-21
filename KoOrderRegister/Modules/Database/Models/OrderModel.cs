using KoOrderRegister.Utility;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Database.Models
{
    [Table("Orders")]
    public class OrderModel
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public string Id { get; init; }
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; } = DateTime.Now;
        [JsonProperty("endOrder")]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }
        [Ignore]
        [JsonIgnore]
        public CustomerModel Customer { get; set; }
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [Ignore]
        [JsonIgnore]
        public List<FileModel> Files { get; set; } = new List<FileModel>();
      
        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("price")]
        public float Price { get; set; }
        [Ignore]
        [JsonIgnore]
        public Guid Guid => Guid.Parse(Id);
        [Ignore]
        [JsonIgnore]
        public string OrderDate => $"{StartDate.ToString(DateFormat.DateTimeFormat)} - {EndDate.ToString(DateFormat.DateTimeFormat)}";

        public OrderModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        public OrderModel(string name): this()
        {
          
        }
    }
}
