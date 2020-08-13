using System.Threading.Tasks;
using WebMVC.Areas.Shelves.Infrastructure;
using WebMVC.Areas.Shelves.ViewModels;
using WebMVC.Infrastructure;

namespace WebMVC.Areas.Shelves.Services
{
    public interface IShelfService
    {
        Task AddBookToShelfAsync(string shelfId, AddBookViewModel model);
        Task AddShelfAsync(UpsertShelfViewModel model);
        Task<bool> DeleteBookFromShelfAsync(string shelfId, long bookId);
        Task<bool> DeleteShelfAsync(string shelfId);
        Task<ShelfDto> GetShelfAsync(string shelfId, int pageIndex = 1,
            int pageSize = 10);
        Task<PaginatedItemsDto<ShelvesViewModel>> GetShelvesAsync(string userId,
            int pageIndex, int pageSize);
        Task<bool> UpdateShelfAsync(string shelfId, UpsertShelfViewModel model);
    }
}
