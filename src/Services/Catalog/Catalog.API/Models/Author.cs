using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookcase.Services.Catalog.API.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }

        public List<BookAuthor> BooksAuthors { get; set; } = new List<BookAuthor>();
    }
}
