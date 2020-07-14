using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookcase.Services.Catalog.API.Models
{
    public class Book
    {
        public long Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public short NumberOfPages { get; set; }

        [Required]
        public string ISBN { get; set; }

        public List<BookAuthor> BooksAuthors { get; set; } = new List<BookAuthor>();
    }
}
