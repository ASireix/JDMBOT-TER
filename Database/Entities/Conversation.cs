using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotJDM.Database.Entities;

namespace BotJDM.Database.Entities
{
    public class Conversation
    {
        [Key]
        public int ConversationId { get; set; }

        [ForeignKey("User")]
        public long? DiscordId { get; set; }
        public required User User { get; set; }

        [ForeignKey("Node1")]
        public int? Node1Id { get; set; }
        public required Node Node1 { get; set; }

        [ForeignKey("Node2")]
        public int? Node2Id { get; set; }
        public required Node Node2 { get; set; }

        [ForeignKey("Relation")]
        public int? RelationId { get; set; }
        public required Relation Relation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
