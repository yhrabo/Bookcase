using System.Collections.Generic;
using System.ComponentModel;

namespace WebMVC.Areas.Catalog.ViewModels
{
    public class BookOutputViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }

        [DisplayName("Number of pages")]
        public short NumberOfPages { get; set; }

        [DisplayName("ISBN")]
        public string Isbn { get; set; }

        public IEnumerable<AuthorOutputViewModel> Authors { get; set; }
    }
}
