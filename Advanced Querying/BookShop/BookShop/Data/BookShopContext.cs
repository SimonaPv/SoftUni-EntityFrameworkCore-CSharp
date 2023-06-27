using BookShop.Data.Models;
using BookShop.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Data
{
    public class BookShopContext : DbContext
    {
        public BookShopContext() { }

        public BookShopContext(DbContextOptions options) : base(options) { }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookCategory> BooksCategories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.connString)
                    .UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity
                .Property(p => p.ReleaseDate)
                .HasColumnType("date");
            });

            modelBuilder.Entity<BookCategory>(entity =>
            {
                entity.HasKey(pk => new { pk.BookId, pk.CategoryId });
            });
        }
    }
}