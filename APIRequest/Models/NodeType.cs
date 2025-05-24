using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BotJDM.APIRequest.Models
{
    public class NodeType
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; } = string.Empty;

        [JsonPropertyName("help")]
        public string help { get; set; } = string.Empty;
    }
}
