using BookShop.Data.Models.Enums;
using BookShop.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Data.Models
{
    public class Book
    {
        public Book()
        {
            this.BookCategories = new HashSet<BookCategory>();
        }

        [Key]
        public int BookId { get; set; }

        [Unicode]
        [MaxLength(ValidationConstraints.BookTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Unicode]
        [MaxLength(ValidationConstraints.BookDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        public DateTime? ReleaseDate { get; set; }

        public int Copies { get; set; }

        public decimal Price { get; set; }

        public EditionType EditionType { get; set; }

        public AgeRestriction AgeRestriction { get; set; }

        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        public virtual ICollection<BookCategory> BookCategories { get; set; }
    }
}
