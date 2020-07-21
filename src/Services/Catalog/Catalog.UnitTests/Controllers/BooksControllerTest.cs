using Bookcase.Services.Catalog.API.Controllers;
using Bookcase.Services.Catalog.API.Infrastructure;
using Bookcase.Services.Catalog.API.Models;
using Bookcase.Services.Catalog.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests.Controllers
{
    [Collection("Collection One.")]
    public class BooksControllerTest
    {
        private readonly DbContextOptions<CatalogDbContext> _dbOptions;

        public BooksControllerTest()
        {
            _dbOptions = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseInMemoryDatabase("inmemory").Options;

            using var ctx = new CatalogDbContext(_dbOptions);
            ctx.Database.EnsureDeleted();

            var authors = GetFakeAuthorsCatalog();
            ctx.Authors.AddRange(authors);
            var books = GetFakeBooksCatalog();
            ctx.Books.AddRange(books);
            AssignFakeBookAuthorCatalogToBooks(authors, books);
            ctx.SaveChanges();
        }

        [Fact]
        public async Task Get_BookById_Success200()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new BooksController(ctx);
            var id = 1;

            // Act.
            var result = await controller.GetBookById(id);

            // Assert.
            var b = Assert.IsAssignableFrom<BookOutputViewModel>(result.Value);
            Assert.Equal(id, b.Id);
            Assert.Equal("Title", b.Title);
            Assert.Equal("1", b.ISBN);
            Assert.Equal(52, b.NumberOfPages);
            Assert.Contains(b.Authors, a => a.Name == "Peter");
        }

        [Fact]
        public async Task Get_BookById_NotFound404()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new BooksController(ctx);
            var id = 200;

            // Act.
            var result = await controller.GetBookById(id);

            // Assert.
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Get_BookById_BadRequest400()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new BooksController(ctx);
            var id = -2;

            // Act.
            var result = await controller.GetBookById(id);

            // Assert.
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task Get_Books_Success200()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new BooksController(ctx);
            var pageSize = 2;
            var pageIndex = 2;

            // Act.
            var result = await controller.GetBooks(pageSize, pageIndex);

            // Assert.
            var pi = Assert.IsType<PaginatedItemsViewModel<BookOutputViewModel>>(result.Value);
            Assert.Equal(pageSize, pi.PageSize);
            Assert.Equal(pageSize, pi.Data.Count());
            Assert.Equal(pageIndex, pi.PageIndex);
            Assert.Contains(pi.Data, i => new long[] { 1, 2 }.Contains(i.Id));
            Assert.Equal(GetFakeBooksCatalog().Count, pi.Count);
            var b = pi.Data.First();
            Assert.Equal("Next title", b.Title);
            Assert.Equal("2", b.ISBN);
            Assert.Equal(121, b.NumberOfPages);
            Assert.Equal(2L, b.Id);
            Assert.Equal("Stacy", b.Authors.First().Name);
            Assert.Equal("Johan", b.Authors.Last().Name);
        }

        [Fact]
        public async Task Get_Books_Empty200()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new BooksController(ctx);
            var pageSize = 20;
            var pageIndex = 20;

            // Act.
            var result = await controller.GetBooks(pageSize, pageIndex);

            // Assert.
            var pi = Assert.IsType<PaginatedItemsViewModel<BookOutputViewModel>>(result.Value);
            Assert.Equal(pageSize, pi.PageSize);
            Assert.Equal(pageIndex, pi.PageIndex);
            Assert.Empty(pi.Data);
            Assert.Equal(GetFakeBooksCatalog().Count, pi.Count);
        }

        [Fact]
        public async Task Post_CreateBook_Success201()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new BooksController(ctx);
            var t = "201Title";
            var isbn = "2123213";
            var numPages = (short)600;
            var authorIds = new int[] { 100, 200 };
            var inputVM = new BookInputViewModel
            { Title = t, ISBN = isbn, NumberOfPages = numPages, AuthorsIds = authorIds };

            // Act.
            var result = await controller.CreateBookAsync(inputVM);

            // Assert.
            var ar = Assert.IsType<CreatedAtActionResult>(result.Result);
            var vm = Assert.IsType<BookOutputViewModel>(ar.Value);
            Assert.Equal(t, vm.Title);
            Assert.Equal(isbn, vm.ISBN);
            Assert.Equal(numPages, vm.NumberOfPages);
            var fb = ctx.Books.Find(vm.Id);
            Assert.NotEqual(0, fb.Id);
            Assert.Contains(fb.BooksAuthors, ba => authorIds.Contains(ba.AuthorId));
        }

        [Fact]
        public async Task Put_Book_Success204()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new BooksController(ctx);
            var id = 2L;
            var t = "201Title";
            var isbn = "2123213";
            var numPages = (short)220;
            var authorIds = new int[] { 50, 51 };
            var inputVM = new BookInputViewModel
            { Title = t, ISBN = isbn, NumberOfPages = numPages, AuthorsIds = authorIds };

            // Act.
            var result = await controller.UpdateBookAsync(id, inputVM);

            // Assert.
            Assert.IsType<NoContentResult>(result);
            var fb = ctx.Books.Find(id);
            Assert.Equal(t, fb.Title);
            Assert.Equal(isbn, fb.ISBN);
            Assert.Equal(numPages, fb.NumberOfPages);
            Assert.Contains(fb.BooksAuthors, ba => authorIds.Contains(ba.AuthorId));
        }

        [Fact]
        public async Task Put_Book_BadRequest400()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new BooksController(ctx);
            var id = 0;
            var upd = new BookInputViewModel();

            // Act.
            var result = await controller.UpdateBookAsync(id, upd);

            // Assert.
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Put_Book_NotFound404()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new BooksController(ctx);
            var id = 200;
            var upd = new BookInputViewModel();

            // Act.
            var result = await controller.UpdateBookAsync(id, upd);

            // Assert.
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Book_Success204()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new BooksController(ctx);
            var id = 2;

            // Act.
            var result = await controller.Delete(id);

            // Assert.
            Assert.IsType<NoContentResult>(result);
            Assert.Null(ctx.Books.SingleOrDefault(a => a.Id == id));
        }

        [Fact]
        public async Task Delete_Book_BadRequest400()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new BooksController(ctx);
            var id = 0;

            // Act.
            var result = await controller.Delete(id);

            // Assert.
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_Book_NotFound404()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new BooksController(ctx);
            var id = 200;

            // Act.
            var result = await controller.Delete(id);

            // Assert.
            Assert.IsType<NotFoundResult>(result);
        }

        private IList<Book> GetFakeBooksCatalog()
        {
            return new Book[]
            {
                new Book { Id = 1, Title = "Title", ISBN = "1", NumberOfPages = 52 },
                new Book { Id = 2, Title = "Next title", ISBN = "2", NumberOfPages = 121 },
                new Book { Id = 3, Title = "Good title", ISBN = "3", NumberOfPages = 100 },
                new Book { Id = 4, Title = "Incredible T", ISBN = "4", NumberOfPages = 102 },
                new Book { Id = 5, Title = "Here?", ISBN = "2-32", NumberOfPages = 434 },
                new Book { Id = 6, Title = "Yes", ISBN = "0-1-2-3", NumberOfPages = 278 }
            };
        }

        private IList<Author> GetFakeAuthorsCatalog()
        {
            return new Author[]
            {
                new Author { Name = "Peter" },
                new Author { Name = "Stacy" },
                new Author { Name = "Johan" },
                new Author { Name = "Mike" },
                new Author { Name = "Anna" },
                new Author { Name = "Michele" },
            };
        }

        private void AssignFakeBookAuthorCatalogToBooks(IList<Author> authors, IList<Book> books)
        {
            var ba = new BookAuthor[]
            {
                new BookAuthor { AuthorId = authors[0].Id, Book = books[0] },
                new BookAuthor { AuthorId = authors[1].Id, Book = books[1] },
                new BookAuthor { AuthorId = authors[2].Id, Book = books[1] },
                new BookAuthor { AuthorId = authors[2].Id, Book = books[2] },
                new BookAuthor { AuthorId = authors[3].Id, Book = books[3] },
                new BookAuthor { AuthorId = authors[2].Id, Book = books[3] },
                new BookAuthor { AuthorId = authors[4].Id, Book = books[4] },
                new BookAuthor { AuthorId = authors[5].Id, Book = books[5] },
            };
            books[0].BooksAuthors.Add(ba[0]);
            books[1].BooksAuthors.Add(ba[1]);
            books[1].BooksAuthors.Add(ba[2]);
            books[2].BooksAuthors.Add(ba[3]);
            books[3].BooksAuthors.Add(ba[4]);
            books[3].BooksAuthors.Add(ba[5]);
            books[4].BooksAuthors.Add(ba[6]);
            books[5].BooksAuthors.Add(ba[7]);
        }
    }
}
