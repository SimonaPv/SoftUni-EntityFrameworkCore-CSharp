using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artillery.Data.Models
{
    public class Manufacturer
    {
        public Manufacturer()
        {
            this.Guns = new HashSet<Gun>();
        }

        [Key]
        public int Id { get; set; }

        [Required] //4, 40
        public string ManufacturerName { get; set; }

        [Required] //10, 100
        public string Founded { get; set; }

        public ICollection<Gun> Guns { get; set; }
    }
}
