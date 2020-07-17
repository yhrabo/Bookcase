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
    public class AuthorsControllerTest
    {
        private readonly DbContextOptions<CatalogDbContext> _dbOptions;

        public AuthorsControllerTest()
        {
            _dbOptions = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseInMemoryDatabase("inmemory").Options;

            using var ctx = new CatalogDbContext(_dbOptions);
            ctx.Database.EnsureDeleted();
            ctx.AddRange(GetFakeAuthorsCatalog());
            ctx.SaveChanges();
        }

        [Fact]
        public async Task Get_AuthorById_Success200()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new AuthorsController(ctx);
            var id = 1;

            // Act.
            var result = await controller.GetAuthorById(id);

            // Assert.
            var a = Assert.IsAssignableFrom<AuthorOutputViewModel>(result.Value);
            Assert.Equal(id, a.Id);
            Assert.Equal("Peter", a.Name);
        }

        [Fact]
        public async Task Get_AuthorById_NotFound404()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new AuthorsController(ctx);
            var id = 200;

            // Act.
            var result = await controller.GetAuthorById(id);

            // Assert.
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Get_AuthorById_BadRequest400()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new AuthorsController(ctx);
            var id = 0;

            // Act.
            var result = await controller.GetAuthorById(id);

            // Assert.
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task Get_Authors_Success200()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new AuthorsController(ctx);
            var pageSize = 3;
            var pageIndex = 1;

            // Act.
            var result = await controller.GetAuthorsAsync(pageSize, pageIndex);

            // Assert.
            var pi = Assert.IsType<PaginatedItemsViewModel<AuthorOutputViewModel>>(result.Value);
            Assert.Equal(pageSize, pi.Data.Count());
            Assert.Equal(GetFakeAuthorsCatalog().Count(), pi.Count);
            Assert.Equal(pageSize, pi.PageSize);
            Assert.Equal(pageIndex, pi.PageIndex);
            Assert.Contains(pi.Data, i => new int[] { 4, 5, 6 }.Contains(i.Id));
        }

        [Fact]
        public async Task Get_Authors_Empty200()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new AuthorsController(ctx);
            var pageSize = 20;
            var pageIndex = 10;

            // Act.
            var result = await controller.GetAuthorsAsync(pageSize, pageIndex);

            // Assert.
            var pi = Assert.IsType<PaginatedItemsViewModel<AuthorOutputViewModel>>(result.Value);
            Assert.Empty(pi.Data);
            Assert.Equal(GetFakeAuthorsCatalog().Count(), pi.Count);
            Assert.Equal(pageSize, pi.PageSize);
            Assert.Equal(pageIndex, pi.PageIndex);
        }

        [Fact]
        public async Task Post_CreateAuthor_Success201()
        {
            // Arrange.
            var name = "Test name";
            var a = new AuthorInputViewModel { Name = name };
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new AuthorsController(ctx);

            // Act.
            var result = await controller.CreateAuthorAsync(a);

            // Assert.
            var ar = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedA = Assert.IsType<AuthorOutputViewModel>(ar.Value);
            Assert.Equal(name, returnedA.Name);
            var createdA = ctx.Authors.Find(returnedA.Id);
            Assert.Equal(name, createdA.Name);
        }

        [Fact]
        public async Task Put_Author_Success204()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new AuthorsController(ctx);
            var id = 1;
            var name = "TestS204";
            var upd = new AuthorInputViewModel { Name = name };

            // Act.
            var result = await controller.UpdateAuthorAsync(id, upd);

            // Assert.
            Assert.IsType<NoContentResult>(result);
            var updatedA = ctx.Authors.Find(id);
            Assert.Equal(name, updatedA.Name);
        }

        [Fact]
        public async Task Put_Author_BadRequest400()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new AuthorsController(ctx);
            var id = 0;
            var upd = new AuthorInputViewModel();

            // Act.
            var result = await controller.UpdateAuthorAsync(id, upd);

            // Assert.
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Put_Author_NotFound404()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new AuthorsController(ctx);
            var id = 200;
            var upd = new AuthorInputViewModel();

            // Act.
            var result = await controller.UpdateAuthorAsync(id, upd);

            // Assert.
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Author_Success204()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new AuthorsController(ctx);
            var id = 2;

            // Act.
            var result = await controller.DeleteAuthorAsync(id);

            // Assert.
            Assert.IsType<NoContentResult>(result);
            Assert.Null(ctx.Authors.SingleOrDefault(a => a.Id == id));
        }

        [Fact]
        public async Task Delete_Author_BadRequest400()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new AuthorsController(ctx);
            var id = 0;

            // Act.
            var result = await controller.DeleteAuthorAsync(id);

            // Assert.
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_Author_NotFound404()
        {
            // Arrange.
            var ctx = new CatalogDbContext(_dbOptions);
            var controller = new AuthorsController(ctx);
            var id = 200;

            // Act.
            var result = await controller.DeleteAuthorAsync(id);

            // Assert.
            Assert.IsType<NotFoundResult>(result);
        }

        private IEnumerable<Author> GetFakeAuthorsCatalog()
        {
            return new Author[]
            {
                new Author { Id = 1, Name = "Peter" },
                new Author { Id = 2, Name = "Stacy" },
                new Author { Id = 3, Name = "Johan" },
                new Author { Id = 4, Name = "Mike" },
                new Author { Id = 5, Name = "Anna" },
                new Author { Id = 6, Name = "Michele" },
                new Author { Id = 7, Name = "Michael" },
                new Author { Id = 8, Name = "Bob" },
            };
        }
    }
}
