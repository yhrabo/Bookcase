using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Bookcase.Services.Shelves.API.Models
{
    public class Shelf
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<ShelfItem> ShelfItems { get; set; } = new List<ShelfItem>();
        public string OwnerId { get; set; }
        public AccessLevel AccessLevel { get; set; }
    }

    public enum AccessLevel { All, Private }
}
