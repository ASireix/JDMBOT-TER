using System.ComponentModel.DataAnnotations;

namespace BotJDM.Database.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public long DiscordId { get; set; }

        [Required]
        [MaxLength(255)]
        public required string DiscordUsername { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}