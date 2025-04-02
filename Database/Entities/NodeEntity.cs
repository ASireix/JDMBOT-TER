using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotJDM.Database.Entities
{
    [Table("Nodes")]
    public class NodeEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Type { get; set; }

        public int W { get; set; }

        public int C { get; set; }

        public double Level { get; set; }

        public int? InfoId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime TouchDate { get; set; }
    }
}
