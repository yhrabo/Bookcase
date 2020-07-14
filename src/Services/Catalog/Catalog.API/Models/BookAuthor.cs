namespace Bookcase.Services.Catalog.API.Models
{
    public class BookAuthor
    {
        public long BookId { get; set; }
        public Book Book { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
