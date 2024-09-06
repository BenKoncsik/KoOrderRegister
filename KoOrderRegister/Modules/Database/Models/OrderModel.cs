﻿using Newtonsoft.Json;
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
        public string Id { get; set; }
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; } = DateTime.Now;
        [JsonProperty("endOrder")]
        public DateTime EndOrder { get; set; } = DateTime.Now;

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
        public List<FileModel> Files { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("price")]
        public float Price { get; set; }
        [Ignore]
        [JsonIgnore]
        public Guid Guid => Guid.Parse(Id);

        public OrderModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        public OrderModel(string name): this()
        {
          
        }
    }
}