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
        public DateTime Date { get; set; }
        public string CustomerId { get; set; }
        [Ignore]
        public CustomerModel Customer { get; set; }
        public long ProductId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string OrderReleaseDate { get; set; }
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
