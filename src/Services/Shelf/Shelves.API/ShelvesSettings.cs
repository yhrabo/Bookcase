using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookcase.Services.Shelves.API
{
    public class ShelvesSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ShelvesCollectionName { get; set; }
    }
}
