using System.ComponentModel;

namespace WebMVC.Areas.Shelves.ViewModels
{
    public class ShelvesViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [DisplayName("Access Level")]
        public AccessLevel AccessLevel { get; set; }
    }

    public enum AccessLevel { All, Private }
}
