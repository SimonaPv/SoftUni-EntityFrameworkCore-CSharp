using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.DataProcessor.ImportDto
{
    public class ImportSellerDto
    {
        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string Name { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string Address { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [RegularExpression(@"www\.[A-Za-z0-9-]+\.com")]
        public string Website { get; set; }

        public int[] Boardgames { get; set; }
    }
}
