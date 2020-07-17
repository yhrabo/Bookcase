using Bookcase.Services.Catalog.API.Infrastructure;
using Bookcase.Services.Catalog.API.Models;
using Bookcase.Services.Catalog.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bookcase.Services.Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly CatalogDbContext _catalogContext;

        public AuthorsController(CatalogDbContext context)
        {
            _catalogContext = context;
        }

        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(typeof(AuthorOutputViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorOutputViewModel>> GetAuthorById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var author = await _catalogContext.Authors.SingleOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            return MapAuthorToAuthorOutputViewModel(author);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<AuthorOutputViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedItemsViewModel<AuthorOutputViewModel>>> GetAuthorsAsync(
            [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            var countedItems = await _catalogContext.Authors.CountAsync();
            var pageItems = await _catalogContext.Authors.OrderByDescending(a => a.Id)
                .Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
            return new PaginatedItemsViewModel<AuthorOutputViewModel>(pageIndex, pageSize, countedItems,
                pageItems.Select(i => MapAuthorToAuthorOutputViewModel(i)));
        }

        [HttpPost]
        public async Task<ActionResult<AuthorOutputViewModel>> CreateAuthorAsync([FromBody] AuthorInputViewModel authorToCreate)
        {
            var a = _catalogContext.Authors.Add(new Author());
            a.CurrentValues.SetValues(authorToCreate);
            await _catalogContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAuthorById), new { id = a.Entity.Id }, MapAuthorToAuthorOutputViewModel(a.Entity));
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateAuthorAsync(int id, [FromBody] AuthorInputViewModel authorToUpdate)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var author = await _catalogContext.Authors.SingleOrDefaultAsync(
                a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            _catalogContext.Entry(author).CurrentValues.SetValues(authorToUpdate);
            _catalogContext.Authors.Update(author);
            await _catalogContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteAuthorAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var author = await _catalogContext.Authors.SingleOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            _catalogContext.Authors.Remove(author);
            await _catalogContext.SaveChangesAsync();
            return NoContent();
        }

        private AuthorOutputViewModel MapAuthorToAuthorOutputViewModel(Author a)
        {
            return new AuthorOutputViewModel
            {
                Id = a.Id,
                Name = a.Name
            };
        }
    }
}
