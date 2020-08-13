using System.ComponentModel.DataAnnotations;

namespace WebMVC.Areas.Shelves.ViewModels
{
    public class UpsertShelfViewModel
    {
        [Required]
        [EnumDataType(typeof(AccessLevel))]
        public AccessLevel AccessLevel { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
