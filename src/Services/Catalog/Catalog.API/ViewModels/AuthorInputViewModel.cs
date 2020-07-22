using Bookcase.Services.Catalog.API.Models;
using System.ComponentModel.DataAnnotations;

namespace Bookcase.Services.Catalog.API.ViewModels
{
    public class AuthorInputViewModel
    {
        [Required]
        [StringLength(Author.NameMaxLength, MinimumLength = 1)]
        public string Name { get; set; }
    }
}
