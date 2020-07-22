using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookcase.Services.Catalog.API.Models
{
    public class Author
    {
        public const int NameMaxLength = 50;

        public int Id { get; set; }
        public string Name { get; set; }
        public List<BookAuthor> BooksAuthors { get; set; } = new List<BookAuthor>();
    }
}
