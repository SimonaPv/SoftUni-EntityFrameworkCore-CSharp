using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Data.Models
{
    public class Category
    {
        public Category()
        {
            this.CategoryBooks = new HashSet<BookCategory>();
        }

        [Key]
        public int CategoryId { get; set; }

        [Unicode]
        [MaxLength(ValidationConstraints.CategoryNameMaxLength)]
        public string Name { get; set; } = null!;

        public virtual ICollection<BookCategory> CategoryBooks { get; set; }
    }
}
