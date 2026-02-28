using Library01.Models;
using System.Linq;

namespace Library01.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorsDisplay { get; set; } = string.Empty;
        public string GenresDisplay { get; set; } = string.Empty;
        public int? PublishYear { get; set; }
        public string? ISBN { get; set; }
        public int QuantityInStock { get; set; }

        public static BookViewModel FromBook(Book book)
        {
            return new BookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                PublishYear = book.PublishYear,
                ISBN = book.ISBN,
                QuantityInStock = book.QuantityInStock,
                AuthorsDisplay = book.BookAuthors.Any()
                    ? string.Join(", ", book.BookAuthors.Select(ba => $"{ba.Author.LastName} {ba.Author.FirstName}"))
                    : "—",
                GenresDisplay = book.BookGenres.Any()
                    ? string.Join(", ", book.BookGenres.Select(bg => bg.Genre.Name))
                    : "—"
            };
        }
    }
}
