using System.Collections.Generic;

namespace WebMVC.Areas.Catalog.ViewModels
{
    public class BookOutputViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public short NumberOfPages { get; set; }
        public string Isbn { get; set; }
        public IEnumerable<AuthorOutputViewModel> Authors { get; set; }
    }
}
