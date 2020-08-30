using System.Threading.Tasks;
using WebMVC.Areas.Catalog.ViewModels;
using WebMVC.Infrastructure;

namespace WebMVC.Areas.Catalog.Services
{
    public interface IBookService
    {
        Task AddBookAsync(BookInputViewModel vm);
        Task<bool> DeleteBookAsync(long id);
        Task<BookOutputViewModel> GetBookAsync(long id);
        Task<PaginatedItemsDto<BookOutputViewModel>> GetBooksAsync(int pageIndex, int pageSize);
        Task<bool> UpdateBookAsync(long id, BookInputViewModel vm);
    }
}
