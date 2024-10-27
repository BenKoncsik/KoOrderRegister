using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Modules.Remote.Model
{
    public class ConnectionData
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("server_key")]
        public string ServerKey { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }

    }
}
