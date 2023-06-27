using Boardgames.Data.Models;
using Boardgames.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType("Creator")]
    public class ImportCreatorDto
    {
        [XmlElement("FirstName")]
        [Required]
        [StringLength(7, MinimumLength = 2)]
        public string FirstName { get; set; }

        [XmlElement("LastName")]
        [Required]
        [StringLength(7, MinimumLength = 2)]
        public string LastName { get; set; }

        [XmlArray("Boardgames")]
        public ImportBoardgameDto[] Boardgames { get; set; }
    }

    [XmlType("Boardgame")]
    public class ImportBoardgameDto
    {
        [XmlElement("Name")]
        [Required]
        [StringLength(20, MinimumLength = 10)]
        public string Name { get; set; }

        [XmlElement("Rating")]
        [Required]
        [Range(1.0, 10.0)]
        public double Rating { get; set; }

        [XmlElement("YearPublished")]
        [Required]
        [Range(2018, 2023)]
        public int YearPublished { get; set; }

        [XmlElement("CategoryType")]
        [Required]
        public int CategoryType { get; set; }

        [XmlElement("Mechanics")]
        [Required]
        public string Mechanics { get; set; }
    }
}
