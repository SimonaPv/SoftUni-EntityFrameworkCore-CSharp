using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace __1
{
    public class Car
    {
        [Key]
        public int CarPrimaryId { get; set; }

        //[Required] - задължително трябва да го  има в базата
        //public string Make { get; set; }

        //[Range(10, 20)]
        //public int Age { get; set; }

        //public string BussinessSpecific { get; set; }

        [ForeignKey(nameof(Engine))]
        public int EngineId { get; set; }
        public Engine Engine { get; set; }
    }

    public class Engine
    {
        public int Id { get; set; }
    }
}