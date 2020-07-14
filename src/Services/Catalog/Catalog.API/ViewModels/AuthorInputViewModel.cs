using System.ComponentModel.DataAnnotations;

namespace Bookcase.Services.Catalog.API.ViewModels
{
    public class AuthorInputViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }
    }
}
