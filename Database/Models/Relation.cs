using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.Database.Models
{
    public class Relation
    {
        [Key]
        public int RelationId { get; set; }

        [Required]
        [MaxLength(255)]
        public required string RelationName { get; set; }

        [Required]
        public required string RelationDescription { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
