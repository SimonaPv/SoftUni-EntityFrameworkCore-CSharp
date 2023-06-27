using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theatre.Data.Models
{
    public class Cast
    {
        [Key]
        public int Id { get; set; }

        [Required]//4, 30
        public string FullName { get; set; }

        [Required]
        public bool IsMainCharacter { get; set; }

        [Required]//regex
        //text in the following format: "+44-{2 numbers}-{3 numbers}-{4 numbers}". Valid phone numbers are: +44-53-468-3479, +44-91-842-6054, +44-59-742-3119 (required)
        public string PhoneNumber { get; set; }

        [Required]
        [ForeignKey(nameof(Play))]
        public int PlayId { get; set; }
        public Play Play { get; set; }
    }
}
