using System.Collections.Generic;
using System.ComponentModel;
using WebMVC.Areas.Shelves.Infrastructure;
using WebMVC.ViewModels;

namespace WebMVC.Areas.Shelves.ViewModels
{
    public class ShelfViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }

        [DisplayName("Access Level")]
        public AccessLevel AccessLevel { get; set; }

        [DisplayName("Shelf Items")]
        public IEnumerable<ShelfItem> ShelfItems { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
