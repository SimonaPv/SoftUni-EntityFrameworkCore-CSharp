using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace SoftJail.Data.Models
{
    public class OfficerPrisoner
    {
        [Required]
        [ForeignKey(nameof(Prisoner))]
        public int PrisonerId { get; set; }

        [Required]
        public Prisoner Prisoner { get; set; }

        [Required]
        [ForeignKey(nameof(Officer))]
        public int OfficerId { get; set; }

        [Required]
        public Officer Officer { get; set; }
    }
}
