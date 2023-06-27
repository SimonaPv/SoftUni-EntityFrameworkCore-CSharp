using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Unicode]
        [MaxLength(ValidationConstraints.BookTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Unicode]
        [MaxLength(ValidationConstraints.BookDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        public DateTime? ReleaseDate { get; set; }
    }
}
