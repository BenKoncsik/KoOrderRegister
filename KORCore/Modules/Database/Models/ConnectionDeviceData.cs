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
        [JsonIgnore]
        public DateTime FirstConnectionData { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public string DeviceKey { get; set; }
    }
}
