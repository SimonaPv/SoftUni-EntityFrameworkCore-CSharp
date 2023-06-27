namespace BookShop
{
    using BookShop.Models.Enums;
    using Castle.DynamicProxy;
    using Data;
    using Initializer;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //int input = int.Parse(Console.ReadLine());
            //string input = Console.ReadLine();

            int result = RemoveBooks(db);

            Console.WriteLine(result);
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            context.RemoveRange(books);
            context.SaveChanges();

            return books.Count();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToArray();

            foreach (var b in books)
            {
                b.Price += 5;
            }

            context.SaveChanges();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    CatName = c.Name,
                    Books = c.CategoryBooks
                             .OrderByDescending(b => b.Book.ReleaseDate)
                             .Select(b => new
                             {
                                 BookTitle = b.Book.Title,
                                 BookYear = b.Book.ReleaseDate.Value.Year
                             })
                            .Take(3)
                })
                .ToArray();

            foreach (var c in books)
            {
                sb.AppendLine($"--{c.CatName}");

                foreach (var b in c.Books)
                {
                    sb.AppendLine($"{b.BookTitle} ({b.BookYear})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var books = context.Categories
                .OrderByDescending(c => c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price))
                .ThenBy(c => c.Name)
                .Select(c => $"{c.Name} ${c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price):f2}")
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var copies = context.Authors
                .OrderByDescending(c => c.Books.Sum(s => s.Copies))
                .Select(c => $"{c.FirstName} {c.LastName} - {c.Books.Sum(s => s.Copies)}")
                .ToArray();

            return string.Join(Environment.NewLine, copies);
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int count = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();
            return count;
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})")
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var titles = context.Books
                .Where(t => t.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(t => t.Title)
                .Select(t => t.Title)
                .ToArray();

            return string.Join(Environment.NewLine, titles);
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .Select(a => $"{a.FirstName} {a.LastName}")
                .ToArray();

            return string.Join(Environment.NewLine, authors);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime d = DateTime.ParseExact(date, "dd-MM-yyyy", null);

            var books = context.Books
                .Where(b => b.ReleaseDate < d)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}")
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input
                .ToLower()
                .Split()
                .ToArray();

            string[] bookTitlesByCategories = context.BooksCategories
                .Where(bc => categories.Contains(bc.Category.Name.ToLower()))
                .Select(bc => bc.Book.Title)
                .OrderBy(t => t)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitlesByCategories);
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            bool ag = Enum.TryParse(typeof(AgeRestriction), command, true, out object objAgeR);

            AgeRestriction ageRestriction;

            if (ag)
            {
                ageRestriction = (AgeRestriction)objAgeR;

                var bookTitles = context.Books
                    .Where(b => b.AgeRestriction == ageRestriction)
                    .OrderBy(b => b.Title)
                    .Select(b => b.Title)
                    .ToArray();

                return string.Join(Environment.NewLine, bookTitles);
            }

            return null;
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var bookTitles = context.Books
                .Where(b => b.Copies < 5000 && b.EditionType == EditionType.Gold)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => $"{b.Title} - ${b.Price:f2}")
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            string[] booksNotReleasedIn = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, booksNotReleasedIn);
        }
    }
}


