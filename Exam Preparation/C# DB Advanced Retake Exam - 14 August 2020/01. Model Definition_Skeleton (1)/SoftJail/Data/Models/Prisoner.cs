using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftJail.Data.Models
{
    public class Prisoner
    {
        public Prisoner()
        {
            this.PrisonerOfficers = new HashSet<OfficerPrisoner>();
            this.Mails = new HashSet<Mail>();
        }

        [Key]
        public int Id { get; set; }

        [Required] //min 3, max 20
        public string FullName { get; set; } 

        [Required] //regex
        public string Nickname { get; set; }

        [Required] //range[18, 65]
        public int Age { get; set; }

        [Required]
        public DateTime IncarcerationDate { get; set; }

        public DateTime? ReleaseDate { get; set; }

        //min 0
        public decimal? Bail { get; set; }

        [Required]
        [ForeignKey(nameof(Cell))]
        public int? CellId { get; set; }

        public Cell? Cell { get; set; }

        public ICollection<Mail> Mails { get; set; }

        public ICollection<OfficerPrisoner> PrisonerOfficers { get; set; }
    }
}
