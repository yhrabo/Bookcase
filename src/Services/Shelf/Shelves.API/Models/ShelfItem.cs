using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookcase.Services.Shelves.API.Models
{
    public class ShelfItem
    {
        public long BookId { get; set; }
        public string BookTitle { get; set; }
        public IEnumerable<string> AuthorName { get; set; } = new List<string>();
    }
}
