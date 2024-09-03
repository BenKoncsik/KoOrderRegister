using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Database.Models
{
    public class OrderModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public CustomerModel Customer { get; set; }
        public long ProductId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string OrderReleaseDate { get; set; }
        public Guid[] FileId { get; set; }
        public List<FileModel> Files { get; set; }
        public string Note { get; set; }
    }
}
