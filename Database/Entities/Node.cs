using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.Database.Entities
{
    public class Node
    {
        [Key]
        public int NodeId { get; set; }

        [Required]
        [MaxLength(255)]
        public required string NodeName { get; set; }

        [Required]
        [MaxLength(255)]
        public required string NodeType { get; set; }

        [Required]
        public required string Attributes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
