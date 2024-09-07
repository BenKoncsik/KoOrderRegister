using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Database.Models
{
    [Table("Customers")]
    public class CustomerModel
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("phone")]
        [DefaultValue("unknown")]
        public string Phone { get; set; }

        [JsonProperty("email")]
        [DefaultValue("unknown@example.com")]
        public string Email { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; } = string.Empty;

        [JsonProperty("nationalHealthInsurance")]
        [DefaultValue("unknown")]
        public string NationalHealthInsurance { get; set; }
        [Ignore]
        [JsonIgnore]
        public List<OrderModel> Orders { get; set; } = new List<OrderModel>();
        [Ignore]
        [JsonIgnore]
        public Guid Guid => Guid.Parse(Id);
        public CustomerModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        public CustomerModel(string name, string address, string phone, string email, string nhi): this()
        {
            Name = name;
            Address = address;
            Phone = phone;
            Email = email;
            NationalHealthInsurance = nhi;
        }

        
    }
}
