using System.Collections.Generic;
using System.ComponentModel;
using WebMVC.Areas.Shelves.ViewModels;
using WebMVC.Infrastructure;

namespace WebMVC.Areas.Shelves.Infrastructure
{
    public class ShelfDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public PaginatedItemsDto<ShelfItem> ShelfItems { get; set; }
    }

    public class ShelfItem
    {
        [DisplayName("Book ID")]
        public long BookId { get; set; }
        [DisplayName("Book Title")]
        public string BookTitle { get; set; }
        public IEnumerable<string> AuthorName { get; set; }
    }
}
