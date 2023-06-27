using SoftJail.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportPrisMailDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [RegularExpression("The [A-Z][a-z]+")]
        public string Nickname { get; set; }

        [Required] 
        [Range(18, 65)]
        public int Age { get; set; }

        [Required]
        public string IncarcerationDate { get; set; }

        public string? ReleaseDate { get; set; }

        [Range(0, int.MaxValue)]
        public decimal? Bail { get; set; }

        [Required]
        [ForeignKey(nameof(Cell))]
        public int? CellId { get; set; }

        public MailDto[] Mails { get; set; }
    }

    public class MailDto
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9 ]+str\.")]
        public string Address { get; set; }
    }
}
