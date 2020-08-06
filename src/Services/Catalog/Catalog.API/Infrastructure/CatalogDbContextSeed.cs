using Bookcase.Services.Catalog.API.Infrastructure;
using Bookcase.Services.Catalog.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Catalog.API.Infrastructure
{
    public class CatalogDbContextSeed
    {
        public void Seed(CatalogDbContext ctx)
        {
            if (ctx.Authors.Any())
                return;

            var authors = new Author[]
            {
                new Author { Name = "Peter" },
                new Author { Name = "Stacy" },
                new Author { Name = "Johan" },
                new Author { Name = "Mike" },
                new Author { Name = "Anna" },
                new Author { Name = "Michele" },
            };
            ctx.Authors.AddRange(authors);
            ctx.SaveChanges();

            var strategy = ctx.Database.CreateExecutionStrategy();
            strategy.Execute(() =>
            {
                using var transaction = ctx.Database.BeginTransaction();
                var books = new Book[]
                {
                    new Book { Id = 1, Title = "Title", Isbn = "9780394415760", NumberOfPages = 52 },
                    new Book { Id = 2, Title = "Next title", Isbn = "9780394415761", NumberOfPages = 121 },
                    new Book { Id = 3, Title = "Good title", Isbn = "9780394415762", NumberOfPages = 100 },
                    new Book { Id = 4, Title = "Incredible T", Isbn = "9780394415763", NumberOfPages = 102 },
                    new Book { Id = 5, Title = "Here?", Isbn = "9780394415764", NumberOfPages = 434 },
                    new Book { Id = 6, Title = "Yes", Isbn = "9780394415765", NumberOfPages = 278 }
                };

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
                ctx.Books.AddRange(books);
                ctx.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Books ON");
                ctx.SaveChanges();
                ctx.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Books OFF");
                transaction.Commit();
            });
        }
    }
}
