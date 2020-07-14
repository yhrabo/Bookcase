using System.Collections.Generic;

namespace Bookcase.Services.Catalog.API.ViewModels
{
    public class PaginatedItemsViewModel<TItem> where TItem : class
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public long Count { get; private set; }
        public IEnumerable<TItem> Data { get; private set; }

        public PaginatedItemsViewModel(int index, int size, long count, IEnumerable<TItem> data)
        {
            PageIndex = index;
            PageSize = size;
            Count = count;
            Data = data;
        }
    }
}
