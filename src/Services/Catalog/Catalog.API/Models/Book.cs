using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bookcase.Services.Catalog.API.Models
{
    public class Book
    {
        private const string IsbnPattern = @"^(97(8|9))?\d{10}$";
        private string _isbn;
        public const int TitleMaxLength = 200;
        public const int IsbnMatchTimeoutInMilliseconds = 100;

        public long Id { get; set; }
        public string Title { get; set; }
        public short NumberOfPages { get; set; }
        public string Isbn {
            get { return _isbn; }
            set
            {
                value = value.Replace(" ", string.Empty);
                value = value.Replace("-", string.Empty);
                if (Regex.IsMatch(value, IsbnPattern, RegexOptions.Compiled,
                    new TimeSpan(0, 0, 0, 0, IsbnMatchTimeoutInMilliseconds)))
                {
                    _isbn = value;
                }
                else
                {
                    throw new ArgumentException("Provided ISBN is invalid");
                }
            }
        }
        public List<BookAuthor> BooksAuthors { get; set; } = new List<BookAuthor>();
    }
}
