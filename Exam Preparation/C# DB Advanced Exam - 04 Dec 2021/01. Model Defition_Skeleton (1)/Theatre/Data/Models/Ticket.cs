using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Theatre.Data.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]//1.0, 100.0
        public decimal Price { get; set; }

        [Required]//1, 10
        public sbyte RowNumber { get; set; }

        [Required]
        [ForeignKey(nameof(Play))]    
        public int PlayId { get; set; }
        public Play Play { get; set; }

        [Required]
        [ForeignKey(nameof(Theatre))]
        public int TheatreId { get; set; }
        public Theatre Theatre { get; set; }
    }
}
