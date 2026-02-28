using System.Collections.Generic;

namespace Library01.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? PublishYear { get; set; }
        public string? ISBN { get; set; }
        public int QuantityInStock { get; set; }

        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
    }
}
