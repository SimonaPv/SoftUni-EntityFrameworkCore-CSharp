﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trucks.Data.Models.Enums;

namespace Trucks.Data.Models
{
    public class Truck
    {
        public Truck()
        {
            this.ClientsTrucks  = new HashSet<ClientTruck>();
        }

        [Key]
        public int Id { get; set; }

        public string RegistrationNumber { get; set; } //length 8, regex

        [Required] //length 17
        public string VinNumber { get; set; }

        public int TankCapacity { get; set; }//range 950, 1420
        public int CargoCapacity { get; set; }//range 5000, 29000

        [Required]
        public CategoryType CategoryType { get; set; }

        [Required]
        public MakeType MakeType { get; set; }

        [Required]
        [ForeignKey(nameof(Despatcher))]
        public int DespatcherId { get; set; }
        public Despatcher Despatcher { get; set; }

        public ICollection<ClientTruck> ClientsTrucks { get; set; }
    }
}
