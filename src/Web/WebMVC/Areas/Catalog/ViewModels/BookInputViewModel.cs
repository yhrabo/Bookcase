using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Areas.Catalog.ViewModels
{
    public class BookInputViewModel
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; }

        [DisplayName("Number of pages")]
        [Range(1, 10000)]
        public short NumberOfPages { get; set; }

        [DisplayName("ISBN")]
        [Required]
        [RegularExpression(@"^[\d\s-]{10,17}$", MatchTimeoutInMilliseconds = 500)]
        public string Isbn { get; set; }

        [DisplayName("Authors")]
        [Required]
        public int[] AuthorsIds { get; set; }
    }
}
