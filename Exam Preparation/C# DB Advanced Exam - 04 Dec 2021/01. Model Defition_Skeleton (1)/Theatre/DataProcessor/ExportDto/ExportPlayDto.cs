using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Theatre.Data.Models.Enums;

namespace Theatre.DataProcessor.ExportDto
{
    [XmlType("Play")]
    public class ExportPlayDto
    {
        [XmlAttribute("Title")]
        [Required]
        public string Title { get; set; }

        [XmlAttribute("Duration")]
        [Required]
        public string Duration { get; set; }

        [XmlAttribute("Rating")]
        [Required]
        public string Rating { get; set; }

        [XmlAttribute("Genre")]
        [Required]
        public string Genre { get; set; }

        [XmlArray("Actors")]
        public ActorDto[] Actors { get; set; }
    }

    [XmlType("Actor")]
    public class ActorDto
    {
        [XmlAttribute("FullName")]
        public string FullName { get; set; }

        [Required]
        [XmlAttribute("MainCharacter")]
        public string MainCharacter { get; set; }
    }
}
