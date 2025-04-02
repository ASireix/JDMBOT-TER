using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.Database.Entities
{
    [Table("Relations")]
    public class RelationEntity
    {
        [Key]
        public int Id { get; set; }

        public int Node1 { get; set; }

        public int Node2 { get; set; }

        public int Type { get; set; }

        public double W { get; set; }

        public double C { get; set; }

        public int InfoId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime TouchDate { get; set; }

        public double Nw { get; set; }

        public int Probability { get; set; }
    }
}
