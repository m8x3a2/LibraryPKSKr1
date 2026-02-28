using System.Collections.Generic;

namespace Library01.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int? BirthYear { get; set; }
        public string? Country { get; set; }

        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

        public override string ToString() => $"{LastName} {FirstName}";
    }
}
