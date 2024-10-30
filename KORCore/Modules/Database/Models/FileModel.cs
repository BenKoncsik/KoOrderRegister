using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Modules.Database.Models
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
        public byte[] Content { get; set; } = null;
        [Ignore]
        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string ContentBase64
        {
            get => Content == null ? null : Convert.ToBase64String(Content);
            set => Content = string.IsNullOrEmpty(value) ? null : Convert.FromBase64String(value);
        }
        [Ignore]
        [JsonIgnore]
        /// <summary>
        /// Microsoft.Maui.Storage.FileResult
        /// </summary>
        public object FileResult { get; set; } = null;
        [JsonProperty("content_type")]
        public string ContentType { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; } = string.Empty;

        [JsonProperty("order_id")]
        public string OrderId { get; set; } = string.Empty;

        [JsonProperty("hash_code")]
        public string HashCode { get; set; } = string.Empty;
        [Ignore]
        [JsonIgnore]
        public string FilePath { get; set; } = string.Empty;
        [Ignore]
        [JsonProperty("order", NullValueHandling = NullValueHandling.Ignore)]
        public OrderModel Order { get; set; }
        [Ignore]
        [JsonIgnore]
        public Guid Guid => Guid.Parse(Id);
        [Ignore]
        [JsonProperty("is_database_file")]
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
