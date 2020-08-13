using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using WebMVC.Areas.Shelves.Infrastructure;
using WebMVC.Areas.Shelves.Services;
using WebMVC.Areas.Shelves.ViewModels;
using WebMVC.ViewModels;

namespace WebMVC.Areas.Shelves.Controllers
{
    [Area("Shelves")]
    [Authorize(AuthenticationSchemes = "OpenIdConnect")]
    public class ShelvesController : Controller
    {
        private readonly IShelfService _shelfService;

        public ShelvesController(IShelfService shelfService)
        {
            _shelfService = shelfService;
        }

        [Route("/shelves/user/{userId}", Name = "UserShelves")]
        // TODO Enable route below.
        //[Route("/shelves/index", Name = "IndexList")]
        [AllowAnonymous]
        public async Task<ActionResult> Index(string userId, int pageIndex = 1, int pageSize = 10)
        {
            var items = await _shelfService.GetShelvesAsync(userId, pageIndex, pageSize);
            var vm = new IndexViewModel<ShelvesViewModel>
            {
                Items = items.Data,
                PaginationInfo = GetPaginationInfo(pageIndex, pageSize, items.Count)
            };
            ViewData["userId"] = userId;
            return View(vm);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Details(string shelfId, int pageIndex = 1, int pageSize = 10)
        {
            var sh = await _shelfService.GetShelfAsync(shelfId, pageIndex, pageSize);
            if (sh == null)
            {
                return NotFound();
            }
            return View(MapShelfDtoToShelfViewModel(sh));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreatePostAction(UpsertShelfViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _shelfService.AddShelfAsync(model);
                TempData["message"] = $"Shelf \"{model.Name}\" was added.";
                return RedirectToAction(nameof(Create));
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(string shelfId)
        {
            var sh = await _shelfService.GetShelfAsync(shelfId, 1, 0);
            if (sh == null)
            {
                return NotFound();
            }
            return View(MapShelfDtoToShelfViewModel(sh));
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPostAction(string shelfId, UpsertShelfViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _shelfService.UpdateShelfAsync(shelfId, model))
                {
                    TempData["message"] = $"Shelf {model.Name} was successfully updated.";
                    return RedirectToAction(nameof(Index), new { userId = User.FindFirst(ClaimTypes.NameIdentifier).Value });
                }
                else
                {
                    return NotFound();
                }
            }
            return View(model);
        }

        public async Task<ActionResult> DeleteBook(string shelfId, long bookId)
        {
            if (await _shelfService.DeleteBookFromShelfAsync(shelfId, bookId))
            {
                TempData["message"] = $"Book with id {shelfId} was successfully deleted from the shelf.";
                return RedirectToAction(nameof(Details), new { shelfId });
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<ActionResult> Delete(string shelfId)
        {
            var sh = await _shelfService.GetShelfAsync(shelfId, 1, 1);
            if (sh == null)
            {
                return NotFound();
            }
            return View(MapShelfDtoToShelfViewModel(sh));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeletePostAction(string shelfId)
        {
            if (await _shelfService.DeleteShelfAsync(shelfId))
            {
                var name = TempData["ShelfName"] as string;
                var id = string.IsNullOrEmpty(name) ? shelfId : name;
                TempData["message"] = $"Shelf {id} was successfully deleted.";
                return RedirectToAction(nameof(Index), new { userId = User.FindFirst(ClaimTypes.NameIdentifier).Value });
            }
            else
            {
                return NotFound();
            }
        }

        private ShelfViewModel MapShelfDtoToShelfViewModel(ShelfDto dto)
        {
            var vm = new ShelfViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                OwnerId = dto.OwnerId,
                AccessLevel = dto.AccessLevel
            };
            if (dto.ShelfItems != null)
            {
                vm.ShelfItems = dto.ShelfItems.Data;
                vm.PaginationInfo = GetPaginationInfo(dto.ShelfItems.PageIndex + 1,
                    dto.ShelfItems.PageSize, dto.ShelfItems.Count);
            }
            return vm;
        }

        private PaginationInfo GetPaginationInfo(int pageIndex, int pageSize, long count)
        {
            var totalPagesCount = count / pageSize + (count % pageSize == 0 ? 0 : 1);
            return new PaginationInfo
            {
                CurrentPage = pageIndex,
                IsNextPageExist = !(pageIndex == totalPagesCount),
                IsPreviousPageExist = !(pageIndex == 1),
                TotalPages = totalPagesCount,
                TotalItems = count
            };
        }
    }
}
