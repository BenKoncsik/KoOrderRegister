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
        public string Id { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndOrder { get; set; } = DateTime.Now;
        public string CustomerId { get; set; }
        [Ignore]
        public CustomerModel Customer { get; set; }
        public string ProductId { get; set; }
        public string OrderNumber { get; set; }
        [Ignore]
        public List<FileModel> Files { get; set; }
        public string Note { get; set; }
        [Ignore]
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
