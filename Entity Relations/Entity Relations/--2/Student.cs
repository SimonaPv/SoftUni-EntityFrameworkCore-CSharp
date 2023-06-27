using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace __2
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int AddressId { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set;}
    }
}
