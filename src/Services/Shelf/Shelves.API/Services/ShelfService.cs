using Bookcase.Services.Shelves.API.Infrastructure;
using Bookcase.Services.Shelves.API.Infrastructure.Exceptions;
using Bookcase.Services.Shelves.API.Models;
using Bookcase.Services.Shelves.API.ViewModels;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Bookcase.Services.Shelves.API.Services
{
    /// <summary>
    /// Implements <see cref="IShelfService"/> operations with MongoDb as
    /// a persistent storage.
    /// </summary>
    class ShelfService : IShelfService
    {
        private readonly IMongoCollection<Shelf> _shelves;

        public ShelfService(ShelvesContext context)
            => _shelves = context.Shelves;

        public async Task<string> AddShelfAsync(UpsertShelfViewModel vm, string ownerId)
        {
            var id = ObjectId.GenerateNewId().ToString();
            await _shelves.InsertOneAsync(new Shelf
            {
                Id = id,
                Name = vm.Name,
                AccessLevel = vm.AccessLevel,
                OwnerId = ownerId
            });
            return id;
        }

        public async Task UpdateShelfAsync(UpsertShelfViewModel vm, string id)
        {
            var filter = Builders<Shelf>.Filter.Where(s => s.Id == id);
            var update = Builders<Shelf>.Update
                .Set(s => s.AccessLevel, vm.AccessLevel)
                .Set(s => s.Name, vm.Name);
            await _shelves.FindOneAndUpdateAsync(filter, update);
        }

        public async Task AddBookToShelfAsync(string shelfId, AddBookViewModel viewModel)
        {
            var book = new ShelfItem()
            {
                AuthorName = viewModel.AuthorName,
                BookId = viewModel.BookId,
                BookTitle = viewModel.BookTitle
            };
            var filter = Builders<Shelf>.Filter.Where(s => s.Id == shelfId);
            var update = Builders<Shelf>.Update.AddToSet(s => s.ShelfItems, book);
            await _shelves.FindOneAndUpdateAsync(filter, update);
        }

        public async Task DeleteBookAsync(string shelfId, long bookId)
        {
            var filter = Builders<Shelf>.Filter.Eq(s => s.Id, shelfId);
            var update = Builders<Shelf>.Update.PullFilter(s => s.ShelfItems,
                si => si.BookId == bookId);
            await _shelves.FindOneAndUpdateAsync(filter, update);
        }

        public async Task<bool> DeleteShelfAsync(string id)
        {
            var filter = Builders<Shelf>.Filter.Eq(s => s.Id, id);
            var result = await _shelves.DeleteOneAsync(filter);
            if (result.IsAcknowledged)
            {
                return result.DeletedCount == 1;
            }
            throw new ShelfDomainException("No acknowledgement from database.");
        }

        public async Task<ShelfViewModel> GetShelfAsync(string id, int shelfPage,
            int shelfPageSize, string shelfUserId, UserRelationshipToShelfOwner relation)
        {
            var filter = relation == UserRelationshipToShelfOwner.None
                ? Builders<Shelf>.Filter.Where(s => (s.Id == id)
                    && (s.OwnerId == shelfUserId) && (s.AccessLevel == AccessLevel.All))
                : Builders<Shelf>.Filter.Where(s => (s.Id == id)
                    && (s.OwnerId == shelfUserId));

            var projection = Builders<Shelf>.Projection
                .Include(s => s.Id)
                .Include(s => s.Name)
                .Slice(s => s.ShelfItems, shelfPage * shelfPageSize, shelfPageSize);
            var shelf = await _shelves.Find(filter)
                .Project<Shelf>(projection).SingleOrDefaultAsync();
            if (shelf == null)
            {
                return null;
            }

            var agg = _shelves.Aggregate().Match(s => (s.Id == id) && (s.OwnerId == shelfUserId));
            agg = relation == UserRelationshipToShelfOwner.None
                ? agg.Match(s => s.AccessLevel == AccessLevel.All)
                : agg;
            var bd = await agg.Project(new BsonDocument("count", new BsonDocument("$size", "$ShelfItems")))
                .FirstOrDefaultAsync();
            var countItems = bd.GetValue("count").ToInt32();
            return new ShelfViewModel
            {
                Id = shelf.Id,
                Name = shelf.Name,
                ShelfItems = new PaginatedItemsViewModel<ShelfItem>(shelfPage, shelfPageSize,
                    countItems, shelf.ShelfItems)
            };
        }

        public async Task<PaginatedItemsViewModel<ShelvesViewModel>> GetShelvesAsync(int shelvesPage,
            int shelvesPageSize, string shelfUserId, UserRelationshipToShelfOwner relation)
        {
            var filter = relation == UserRelationshipToShelfOwner.None
                ? Builders<Shelf>.Filter.Where(s =>
                     (s.OwnerId == shelfUserId) && (s.AccessLevel == AccessLevel.All))
                : Builders<Shelf>.Filter.Where(s =>
                    s.OwnerId == shelfUserId);
            var projection = Builders<Shelf>.Projection
                .Include(s => s.Id)
                .Include(s => s.Name)
                .Include(s => s.AccessLevel);

            var shelves = await _shelves.Find(filter).Skip(shelvesPage * shelvesPageSize)
                .Limit(shelvesPageSize).Project<ShelvesViewModel>(projection).ToListAsync();
            if (shelves.Count == 0)
            {
                return null;
            }
            var itemsCount = await _shelves.CountDocumentsAsync(filter);
            return new PaginatedItemsViewModel<ShelvesViewModel>(shelvesPage, shelvesPageSize,
                itemsCount, shelves);
        }

        public async Task<long> CountShelvesAsync()
        {
            return await _shelves.EstimatedDocumentCountAsync();
        }

        public async Task<string> GetShelfOwnerIdAsync(string id)
        {
            var o = await _shelves.AsQueryable().Where(s => s.Id == id)
                .Select(s => new { s.OwnerId }).SingleOrDefaultAsync();
            return o.OwnerId;
        }

        private (bool isAccessLevelValid, AccessLevel al) ChooseAccessLevel(string shelfUserId, string reqUserId)
        {
            return shelfUserId == reqUserId ? (false, AccessLevel.All)
                : (true, AccessLevel.All);
        }
    }

    public enum UserRelationshipToShelfOwner { Owner, None }
}
