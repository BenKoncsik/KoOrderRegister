using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Database.Models
{
    [Table("Customers")]
    public class CustomerModel
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Note { get; set; } = string.Empty;
        public string NationalHealthInsurance { get; set; }
        [Ignore]
        public List<OrderModel> Orders { get; set; } = new List<OrderModel>();
        [Ignore]
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
