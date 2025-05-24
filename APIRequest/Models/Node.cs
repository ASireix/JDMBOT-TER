using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BotJDM.APIRequest.Models
{
    public class Node
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public int type { get; set; }

        [JsonPropertyName("w")]
        public int w { get; set; }

        [JsonPropertyName("c")]
        public int c { get; set; }

        [JsonPropertyName("level")]
        public double level { get; set; }

        [JsonPropertyName("infoid")]
        public int? infoId { get; set; }

        [JsonPropertyName("creationdate")]
        public DateTime creationDate { get; set; }

        [JsonPropertyName("touchdate")]
        public DateTime touchDate { get; set; }
    }
}
