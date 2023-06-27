using BookShop.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [Unicode]
        [MaxLength(ValidationConstraints.AuthorNamesMaxLength)]
        public string? FirstName { get; set; }

        [Unicode]
        [MaxLength(ValidationConstraints.AuthorNamesMaxLength)]
        public string LastName { get; set; } = null!;
    }
}