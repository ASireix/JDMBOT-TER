using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BotJDM.APIRequest.Models
{
    public class RelationType
    {
        [JsonPropertyName("id")]
        public int id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; } = string.Empty;

        [JsonPropertyName("gpname")]
        public string gpName { get; set; } = string.Empty;

        [JsonPropertyName("quot")]
        public double quot { get; set; }

        [JsonPropertyName("quotmin")]
        public double quotMin { get; set; }

        [JsonPropertyName("quotmax")]
        public double quotMax { get; set; }

        [JsonPropertyName("price")]
        public int price { get; set; }

        [JsonPropertyName("help")]
        public string help { get; set; } = string.Empty;

        [JsonPropertyName("playable")]
        public int playable { get; set; }

        [JsonPropertyName("oppos")]
        public int oppos { get; set; }

        [JsonPropertyName("posyes")]
        public string posYes { get; set; } = string.Empty;

        [JsonPropertyName("posno")]
        public string posNo { get; set; } = string.Empty;

        [JsonPropertyName("constraint_ent")]
        public int? constraintEnt { get; set; }

        [JsonPropertyName("constraints_start")]
        public string constraintsStart { get; set; } = string.Empty;

        [JsonPropertyName("constraints_end")]
        public string constraintsEnd { get; set; } = string.Empty;

        [JsonPropertyName("carac")]
        public string carac { get; set; } = string.Empty;
    }
}
