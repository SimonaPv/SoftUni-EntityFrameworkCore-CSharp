using SoftJail.Data.Models.Enums;
using SoftJail.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class ImportOfficerDto
    {
        [XmlElement("Name")]
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string FullName { get; set; }

        [XmlElement("Money")]
        [Required]
        [Range(0, int.MaxValue)]
        public decimal Salary { get; set; }

        [XmlElement("Position")]
        [Required]
        public string Position { get; set; }

        [XmlElement("Weapon")]
        [Required]
        public string Weapon { get; set; }

        [XmlElement("DepartmentId")]
        [Required]
        public int DepartmentId { get; set; }

        [XmlArray("Prisoners")]
        public PrisonerDto[] Prisoners { get; set; }
    }

    [XmlType("Prisoner")]
    public class PrisonerDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
