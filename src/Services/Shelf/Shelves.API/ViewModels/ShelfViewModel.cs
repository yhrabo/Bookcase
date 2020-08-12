using Bookcase.Services.Shelves.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookcase.Services.Shelves.API.ViewModels
{
    public class ShelfViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public string OwnerId { get; set; }
        public PaginatedItemsViewModel<ShelfItem> ShelfItems { get; set; }
    }
}
