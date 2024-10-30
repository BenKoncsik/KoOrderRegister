using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Modules.Database.Models
{
    [Table("ConnectionDeviceDatas")]
    public class ConnectionDeviceData
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public string Id { get; init; }
        [JsonProperty("first_connection_date")]
        public DateTime FirstConnectionData { get; set; } = DateTime.UtcNow;
        [JsonProperty("last_connection_date")]
        public DateTime LastConnectionData { get; set; } = DateTime.UtcNow;
        [JsonProperty("device_key")]
        public string DeviceKey { get; set; }
        [JsonProperty("server_key")]
        public string ServerKey { get; set; }
        public string Url { get; set; }
        [JsonIgnore]
        [Ignore]
        public Guid Guid => Guid.Parse(Id);

        public ConnectionDeviceData()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
