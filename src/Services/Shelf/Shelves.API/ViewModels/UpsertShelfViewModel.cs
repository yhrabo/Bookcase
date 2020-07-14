using Bookcase.Services.Shelves.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bookcase.Services.Shelves.API.ViewModels
{
    public class UpsertShelfViewModel
    {
        [Required]
        public AccessLevel AccessLevel { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
