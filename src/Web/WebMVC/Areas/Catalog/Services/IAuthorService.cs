using System.Threading.Tasks;
using WebMVC.Areas.Catalog.Infrastructure;
using WebMVC.Areas.Catalog.ViewModels;

namespace WebMVC.Areas.Catalog.Services
{
    public interface IAuthorService
    {
        Task<AuthorOutputViewModel> AddAuthorAsync(AuthorInputViewModel vm);
        Task<bool> DeleteAuthorAsync(int id);
        Task<AuthorOutputViewModel> GetAuthorAsync(int id);
        Task<PaginatedItemsDto<AuthorOutputViewModel>> GetAuthorsAsync(int pageIndex, int pageSize);
        Task<bool> UpdateAuthorAsync(int id, AuthorInputViewModel vm);
    }
}
