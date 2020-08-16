using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [DisplayName("User name")]
        public string UserName { get; set; }
    }
}
