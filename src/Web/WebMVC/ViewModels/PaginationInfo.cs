using System.ComponentModel;

namespace WebMVC.ViewModels
{
    public class PaginationInfo
    {
        public long TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public bool IsPreviousPageExist { get; set; }
        public bool IsNextPageExist { get; set; }
        [DisplayName("Total Items")]
        public long? TotalItems { get; set; }
    }
}
