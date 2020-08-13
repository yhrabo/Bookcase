using System.ComponentModel.DataAnnotations;

namespace WebMVC.Areas.Catalog.ViewModels
{
    public class AuthorInputViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }
    }
}
