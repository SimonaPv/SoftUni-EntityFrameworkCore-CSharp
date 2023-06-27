using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artillery.Data.Models
{
    public class Country
    {
        public Country()
        {
            this.CountriesGuns = new HashSet<CountryGun>();
        }

        [Key]
        public int Id { get; set; }

        [Required] //4, 60
        public string CountryName { get; set; }

        [Required] //[50_000….10_000_000]
        public int ArmySize { get; set; }

        public ICollection<CountryGun> CountriesGuns { get; set; }
    }
}
