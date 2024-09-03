using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Database.Models
{
    public class FileModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public string Note { get; set; } = string.Empty;
        public OrderModel Order { get; set; }
        public FileModel()
        {
            Id = Guid.NewGuid();
        }
        public FileModel(string name, byte[] content)
        {
            Id = Guid.NewGuid();
            Name = name;
            Content = content;
        }
    }
}
