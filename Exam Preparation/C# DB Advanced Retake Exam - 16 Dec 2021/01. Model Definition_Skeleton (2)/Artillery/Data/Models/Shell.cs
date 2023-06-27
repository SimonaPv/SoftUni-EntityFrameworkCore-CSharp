﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artillery.Data.Models
{
    public class Shell
    {
        public Shell()
        {
            this.Guns = new HashSet<Gun>();
        }

        [Key]
        public int Id { get; set; }

        [Required]// [2, 1_680] 
        public double ShellWeight { get; set; }

        [Required] //4, 30
        public string Caliber { get; set; }

        public ICollection<Gun> Guns { get; set; }
    }
}