using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Database.Models
{
    public class CustomerModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Note { get; set; } = string.Empty;
        public string NationalHealthInsurance { get; set; }
        public Guid[] OrderId { get; set; }
        public List<OrderModel> Orders { get; set; } = new List<OrderModel>();
        public CustomerModel()
        {
            Id = Guid.NewGuid();
        }

        public CustomerModel(string name, string address, string phone, string email, string nhi)
        {
            Id = Guid.NewGuid();
            Name = name;
            Address = address;
            Phone = phone;
            Email = email;
            NationalHealthInsurance = nhi;
        }
    }
}
