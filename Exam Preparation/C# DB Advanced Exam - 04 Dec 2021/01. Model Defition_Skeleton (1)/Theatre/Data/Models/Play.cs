using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theatre.Data.Models.Enums;

namespace Theatre.Data.Models
{
    public class Play
    {
        public Play()
        {
            this.Casts = new HashSet<Cast>();
            this.Tickets = new HashSet<Ticket>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }//4, 50

        [Required] //min length of 1 hour, format {hours:minutes:seconds}
        public TimeSpan Duration { get; set; }

        [Required]//0.0, 10.0
        public float Rating { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Required]//up to 700
        public string Description { get; set; }

        [Required]//4, 30
        public string Screenwriter { get; set; }

        public ICollection<Cast> Casts { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
