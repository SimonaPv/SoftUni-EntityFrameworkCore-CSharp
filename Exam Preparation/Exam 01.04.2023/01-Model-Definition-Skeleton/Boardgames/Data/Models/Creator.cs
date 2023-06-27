﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.Data.Models
{
    public class Creator
    {
        public Creator()
        {
            this.Boardgames = new HashSet<Boardgame>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(7, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(7, MinimumLength = 2)]
        public string LastName { get; set; }

        public ICollection<Boardgame> Boardgames { get; set; }
    }
}