using System.Collections.Generic;

namespace WebMVC.Areas.Catalog.ViewModels
{
    public class IndexViewModel<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
