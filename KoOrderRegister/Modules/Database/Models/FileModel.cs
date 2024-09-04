using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Database.Models
{
    [Table("Files")]
    public class FileModel
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public string Note { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        [Ignore]
        public string FilePath { get; set; } = string.Empty;
        [Ignore]
        public OrderModel Order { get; set; }
        [Ignore]
        public Guid Guid => Guid.Parse(Id);
        public FileModel()
        {
            Id = Guid.NewGuid().ToString();
        }
        public FileModel(string name, byte[] content) : this()
        {
            Name = name;
            Content = content;
        }
    }
}
