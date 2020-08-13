using System.Collections.Generic;

namespace WebMVC.Infrastructure
{
    public class PaginatedItemsDto<TItem> where TItem : class
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long Count { get; set; }
        public IEnumerable<TItem> Data { get; set; }
    }
}
