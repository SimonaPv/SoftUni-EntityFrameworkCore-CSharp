using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Manufacturer")]
    public class ImportManufacturersDto
    {
        [Required] 
        [StringLength(40, MinimumLength = 4)]
        [XmlElement("ManufacturerName")]
        public string ManufacturerName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 10)]
        [XmlElement("Founded")]
        public string Founded { get; set; }
    }
}
