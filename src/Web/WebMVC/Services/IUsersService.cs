using System.Threading.Tasks;
using WebMVC.Infrastructure;
using WebMVC.ViewModels;

namespace WebMVC.Services
{
    public interface IUsersService
    {
        Task<PaginatedItemsDto<UserViewModel>> GetUsersAsync(int pageIndex, int pageSize);
    }
}
