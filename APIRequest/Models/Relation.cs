using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BotJDM.APIRequest.Models
{
    public class Relation
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("node1")]
        public int node1 { get; set; }

        [JsonPropertyName("node2")]
        public int node2 { get; set; }

        [JsonPropertyName("type")]
        public int type { get; set; }

        [JsonPropertyName("w")]
        public double w { get; set; }

        [JsonPropertyName("c")]
        public double c { get; set; }

        [JsonPropertyName("infoid")]
        public int infoId { get; set; }

        [JsonPropertyName("creationdate")]
        public DateTime creationDate { get; set; }

        [JsonPropertyName("touchdate")]
        public DateTime touchDate { get; set; }

        [JsonPropertyName("nw")]
        public double nw { get; set; }
    }
}
