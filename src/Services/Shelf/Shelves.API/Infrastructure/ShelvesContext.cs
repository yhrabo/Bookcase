using Bookcase.Services.Shelves.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Bookcase.Services.Shelves.API.Infrastructure
{
    public class ShelvesContext
    {
        private readonly IMongoDatabase _database;

        public ShelvesContext(IOptions<ShelvesSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Shelf> Shelves
        {
            get
            {
                return _database.GetCollection<Shelf>("Shelves");
            }
        }
    }
}
