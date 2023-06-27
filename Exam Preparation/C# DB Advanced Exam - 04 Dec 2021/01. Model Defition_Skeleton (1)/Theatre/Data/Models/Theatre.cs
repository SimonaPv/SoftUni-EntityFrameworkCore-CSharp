using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Theatre.Data.Models
{
    public class Theatre
    {
        public Theatre()
        {
            this.Tickets = new HashSet<Ticket>();
        }

        [Key]
        public int Id { get; set; }

        [Required]//4, 30
        public string Name { get; set; }

        [Required]//1, 10
        public sbyte NumberOfHalls { get; set; }

        [Required]//4, 30
        public string Director { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
