using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebMVC.Services;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task<ActionResult> Index(int pageSize = 10, int pageIndex = 1)
        {
            var pi = await _usersService.GetUsersAsync(pageIndex, pageSize);
            var countTotalPages = pi.Count / pageSize + (pi.Count % pageSize == 0 ? 0 : 1);
            var vm = new IndexViewModel<UserViewModel>
            {
                Items = pi.Data,
                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageIndex,
                    TotalPages = countTotalPages,
                    IsNextPageExist = !(pageIndex == countTotalPages),
                    IsPreviousPageExist = !(pageIndex == 1),
                }
            };
            return View(vm);
        }
    }
}
