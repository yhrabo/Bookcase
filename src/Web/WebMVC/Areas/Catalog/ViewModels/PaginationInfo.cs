using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Areas.Catalog.ViewModels
{
    public class PaginationInfo
    {
        public long TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public bool IsPreviousPageExist { get; set; }
        public bool IsNextPageExist { get; set; }
    }
}
