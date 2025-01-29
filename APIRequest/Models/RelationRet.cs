using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BotJDM.APIRequest.Models
{
    public class RelationRet
    {
        [JsonPropertyName("nodes")]
        public List<Node> nodes { get; set; } = new();

        [JsonPropertyName("relations")]
        public List<Relation> relations { get; set; } = new();
    }
}
