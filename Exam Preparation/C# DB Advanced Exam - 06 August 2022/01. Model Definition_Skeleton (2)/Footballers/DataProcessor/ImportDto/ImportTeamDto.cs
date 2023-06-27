using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Footballers.DataProcessor.ImportDto
{
    public class ImportTeamDto
    {
        [Required]
        [StringLength(40, MinimumLength = 2)]
        [RegularExpression(@"[A-Za-z0-9 \.\-]+")]
        public string Name { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string Nationality { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Trophies { get; set; }

        public int[] Footballers { get; set; }
    }
}
