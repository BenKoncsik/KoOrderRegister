using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Module.Database.Models
{
    [Table("Files")]
    public class FileModel
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public string Id { get; init; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonIgnore]
        public byte[] Content { get; set; }
        [JsonProperty("content")]
        [Ignore]
        public string ContentBase64
        {
            get => Content == null ? null : Convert.ToBase64String(Content);
            set => Content = string.IsNullOrEmpty(value) ? null : Convert.FromBase64String(value);
        }
        [Ignore]
        [JsonIgnore]
        public FileResult FileResult { get; set; }
        [JsonProperty("contentType")]
        public string ContentType { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; } = string.Empty;

        [JsonProperty("orderId")]
        public string OrderId { get; set; } = string.Empty;

        [JsonProperty("hashCode")]
        public string HashCode { get; set; } = string.Empty;
        [Ignore]
        [JsonIgnore]
        public string FilePath { get; set; } = string.Empty;
        [Ignore]
        [JsonIgnore]
        public OrderModel Order { get; set; }
        [Ignore]
        [JsonIgnore]
        public Guid Guid => Guid.Parse(Id);
        [Ignore]
        [JsonIgnore]
        public bool IsDatabaseContent { get; set; } = false;
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
