using BookShop.Data;
using BookShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Author
    {
        public Author()
        {
            this.Books = new HashSet<Book>();
        }

        [Key]
        public int AuthorId { get; set; }

        [Unicode]
        [MaxLength(ValidationConstraints.AuthorNamesMaxLength)]
        public string? FirstName { get; set; }

        [Unicode]
        [MaxLength(ValidationConstraints.AuthorNamesMaxLength)]
        public string LastName { get; set; } = null!;

        public virtual ICollection<Book> Books { get; set; }
    }
}