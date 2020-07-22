using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookcase.Services.Catalog.API.Models
{
    public class Book
    {
        public const int TitleMaxLength = 200;

        public long Id { get; set; }
        public string Title { get; set; }
        public short NumberOfPages { get; set; }
        public string Isbn { get; set; }
        public List<BookAuthor> BooksAuthors { get; set; } = new List<BookAuthor>();
    }
}
