using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Areas.Catalog.Infrastructure;
using WebMVC.Areas.Catalog.Services;
using WebMVC.Areas.Catalog.ViewModels;

namespace WebMVC.Areas.Catalog.Controllers
{
    [Area("Catalog")]
    [Authorize(AuthenticationSchemes = "OpenIdConnect")]
    public class AuthorsController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Index(int pageSize = 10, int pageIndex = 1)
        {
            var pi = await _authorService.GetAuthorsAsync(pageIndex, pageSize);
            var countTotalPages = pi.Count / pageSize + (pi.Count % pageSize == 0 ? 0 : 1);
            var vm = new IndexViewModel<AuthorOutputViewModel>
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
            var a = await _authorService.GetAuthorAsync(id);
            if (a == null)
            {
                return NotFound();
            }
            return View(a);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AuthorInputViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var addedA = await _authorService.AddAuthorAsync(vm);
                TempData["message"] = $"Author \"{addedA.Name}\" was added.";
                return RedirectToAction(nameof(Create));
            }
            return View(vm);
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var a = await _authorService.GetAuthorAsync(id);
            if (a == null)
            {
                return NotFound();
            }
            return View(a);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPostAction(int id, AuthorInputViewModel vm)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                if (await _authorService.UpdateAuthorAsync(id, vm))
                {
                    TempData["message"] = $"Author {vm.Name} was successfully updated.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return NotFound();
                }
            }
            return View();
        }

        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var a = await _authorService.GetAuthorAsync(id);
            if (a == null)
            {
                return NotFound();
            }
            return View(a);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeletePostAction(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            if (await _authorService.DeleteAuthorAsync(id))
            {
                TempData["message"] = $"Author with id {id} was successfully deleted.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
