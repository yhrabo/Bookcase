using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebMVC.Areas.Catalog.Services;
using WebMVC.Areas.Catalog.ViewModels;
using WebMVC.Infrastructure;
using WebMVC.ViewModels;

namespace WebMVC.Areas.Catalog.Controllers
{
    [Area("Catalog")]
    [Authorize(AuthenticationSchemes = "OpenIdConnect")]
    public class BooksController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService, IAuthorService authorService)
        {
            _bookService = bookService;
            _authorService = authorService;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Index(int pageSize = 10, int pageIndex = 1)
        {
            var pi = await _bookService.GetBooksAsync(pageIndex, pageSize);
            var countTotalPages = pi.Count / pageSize + (pi.Count % pageSize == 0 ? 0 : 1);
            var vm = new IndexViewModel<BookOutputViewModel>
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

        [AllowAnonymous]
        public async Task<ActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var b = await _bookService.GetBookAsync(id);
            if (b == null)
            {
                return NotFound();
            }
            return View(b);
        }

        public async Task<ActionResult> Create()
        {
            var data = await _authorService.GetAuthorsAsync(1, 1000);
            ViewData["authors"] = data.Data;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BookInputViewModel vm)
        {
            if (ModelState.IsValid)
            {
                await _bookService.AddBookAsync(vm);
                TempData["message"] = $"Book \"{vm.Title}\" was added.";
                return RedirectToAction(nameof(Create));
            }
            var data = await _authorService.GetAuthorsAsync(1, 1000);
            ViewData["authors"] = data.Data;
            return View(vm);
        }

        public async Task<ActionResult> Edit(long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var b = await _bookService.GetBookAsync(id);
            if (b == null)
            {
                return NotFound();
            }
            var data = await _authorService.GetAuthorsAsync(1, 1000);
            ViewData["AuthorsList"] = new MultiSelectList(data.Data, nameof(AuthorOutputViewModel.Id),
                nameof(AuthorOutputViewModel.Name), b.Authors.Select(a => a.Id));
            return View(new BookInputViewModel
            {
                Isbn = b.Isbn,
                Title = b.Title,
                NumberOfPages = b.NumberOfPages,
            });
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPostAction(long id, BookInputViewModel vm)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                if (await _bookService.UpdateBookAsync(id, vm))
                {
                    TempData["message"] = $"Author {vm.Title} was successfully updated.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound();
                }
            }
            var data = await _authorService.GetAuthorsAsync(1, 1000);
            ViewData["AuthorsList"] = new MultiSelectList(data.Data, nameof(AuthorOutputViewModel.Id),
                nameof(AuthorOutputViewModel.Name), vm.AuthorsIds);
            return View(vm);
        }

        public async Task<ActionResult> Delete(long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var b = await _bookService.GetBookAsync(id);
            if (b == null)
            {
                return NotFound();
            }
            return View(b);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeletePostAction(long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            if (await _bookService.DeleteBookAsync(id))
            {
                TempData["message"] = $"Book with id {id} was successfully deleted.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
