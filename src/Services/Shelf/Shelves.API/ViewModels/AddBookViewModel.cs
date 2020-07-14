using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookcase.Services.Shelves.API.ViewModels
{
    public class AddBookViewModel
    {
        [Required]
        public long BookId { get; set; }
        [Required]
        public string BookTitle { get; set; }
        [Required]
        public IEnumerable<string> AuthorName { get; set; }
    }
}
