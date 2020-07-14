namespace Bookcase.Services.Catalog.API.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class BookInputViewModel
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Title { get; set; }

        [Range(1, 10000)]
        public short NumberOfPages { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public int[] AuthorsIds { get; set; }
    }
}
