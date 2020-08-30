using Bookcase.Services.Catalog.API.Infrastructure;
using Bookcase.Services.Catalog.API.Models;
using Bookcase.Services.Catalog.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookcase.Services.Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly CatalogDbContext _catalogContext;

        public BooksController(CatalogDbContext context)
        {
            _catalogContext = context;
        }

        [HttpGet]
        [Route("{id:long}")]
        [ProducesResponseType(typeof(BookOutputViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult<BookOutputViewModel>> GetBookById(long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var book = await _catalogContext.Books.GetBook().SingleOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return MapBookToBookOutputViewModel(book);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<BookOutputViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult<PaginatedItemsViewModel<BookOutputViewModel>>> GetBooks(
            [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var countedItems = await _catalogContext.Books.CountAsync();
            var pageItems = await _catalogContext.Books.GetBook().OrderByDescending(b => b.Id)
                .Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
            return new PaginatedItemsViewModel<BookOutputViewModel>(pageIndex, pageSize, countedItems,
                pageItems.Select(i => MapBookToBookOutputViewModel(i)));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookAsync([FromBody] BookInputViewModel bookToCreate)
        {
            var book = _catalogContext.Books.Add(new Book());
            book.CurrentValues.SetValues(bookToCreate);
            book.Entity.BooksAuthors.AddRange(MapBookAuthorsToBookAuthorEnumerable(book.Entity, bookToCreate));
            await _catalogContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBookById), new { id = book.Entity.Id }, null);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateBookAsync(long id, [FromBody] BookInputViewModel bookToUpdate)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var book = await _catalogContext.Books.GetBook().SingleOrDefaultAsync(
                b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            _catalogContext.Entry(book).CurrentValues.SetValues(bookToUpdate);
            book.BooksAuthors = MapBookAuthorsToBookAuthorEnumerable(book, bookToUpdate);

            // Update book authors.
            var currentBookAuthorsIds = book.BooksAuthors.Select(ba => ba.AuthorId);
            var idsToRemove = currentBookAuthorsIds.Except(bookToUpdate.AuthorsIds);
            foreach (var aId in idsToRemove)
            {
                var toRemove = book.BooksAuthors.FirstOrDefault(b => b.AuthorId == aId);
                if (toRemove != null)
                    book.BooksAuthors.Remove(toRemove);
            }
            var idsToAdd = bookToUpdate.AuthorsIds.Except(currentBookAuthorsIds);
            foreach (var aId in idsToAdd)
            {
                book.BooksAuthors.Add(new BookAuthor
                {
                    AuthorId = aId,
                    Book = book
                });
            }

            _catalogContext.Books.Update(book);
            await _catalogContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var book = await _catalogContext.Books.SingleOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            _catalogContext.Books.Remove(book);
            await _catalogContext.SaveChangesAsync();
            return NoContent();
        }

        private List<BookAuthor> MapBookAuthorsToBookAuthorEnumerable(Book book, BookInputViewModel bookVm)
        {
            var bookAuthors = new List<BookAuthor>();
            foreach (var id in bookVm.AuthorsIds)
            {
                bookAuthors.Add(new BookAuthor
                {
                    AuthorId = id,
                    Book = book
                });
            }
            return bookAuthors;
        }

        private BookOutputViewModel MapBookToBookOutputViewModel(Book b)
        {
            return new BookOutputViewModel
            {
                Id = b.Id,
                Isbn = b.Isbn,
                NumberOfPages = b.NumberOfPages,
                Title = b.Title,
                Authors = b.BooksAuthors.Select(ba => new AuthorOutputViewModel
                {
                    Id = ba.Author.Id,
                    Name = ba.Author.Name
                })
            };
        }
    }

    public static class IQuerybleBook
    {
        public static IQueryable<Book> GetBook(this IQueryable<Book> query)
        {
            return query.Include(b => b.BooksAuthors).ThenInclude(ba => ba.Author);
        }
    }
}
