using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookcase.Services.Catalog.API.ViewModels
{
    public class BookOutputViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public short NumberOfPages { get; set; }
        public string ISBN { get; set; }
        public IEnumerable<AuthorOutputViewModel> Authors { get; set; }
    }
}
