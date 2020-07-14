using Bookcase.Services.Shelves.API.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace Bookcase.Services.Shelves.API.Infrastructure
{
    public class ShelvesContextSeed
    {
        public void Seed(ShelvesContext ctx)
        {
            if (ctx.Shelves.EstimatedDocumentCount() > 0)
                return;

            var shelves = new Shelf[]
            {
                new Shelf
                {
                    Id = "5f06c78c7484300017511baa",
                    Name = "Read",
                    AccessLevel = AccessLevel.All,
                    OwnerId = "a99ade55-6f9e-46e6-9e1d-251d83498d1e",
                    ShelfItems = new List<ShelfItem>
                    {
                        new ShelfItem
                        {
                            BookId = 1,
                            BookTitle = "Title",
                            AuthorName = new List<string> { "Peter" }
                        },
                        new ShelfItem
                        {
                            BookId = 2,
                            BookTitle = "Next title",
                            AuthorName = new List<string> { "Stacy", "Johan" }
                        },
                    }
                },
                new Shelf
                {
                    Id = "5f06c78c7484300017511bab",
                    Name = "To read",
                    AccessLevel = AccessLevel.Private,
                    OwnerId = "a99ade55-6f9e-46e6-9e1d-251d83498d1e",
                    ShelfItems = new List<ShelfItem>
                    {
                        new ShelfItem
                        {
                            BookId = 3,
                            BookTitle = "Good title",
                            AuthorName = new List<string> { "Johan" }
                        },
                    }
                },
                new Shelf
                {
                    Id = "5f06c78c7484300017511bac",
                    Name = "Read",
                    AccessLevel = AccessLevel.Private,
                    OwnerId = "55042217-5552-48bf-88fb-36fef5af23da",
                    ShelfItems = new List<ShelfItem>
                    {
                    }
                },
                new Shelf
                {
                    Id = "5f06c78c7484300017511bad",
                    Name = "To read",
                    AccessLevel = AccessLevel.Private,
                    OwnerId = "55042217-5552-48bf-88fb-36fef5af23da",
                    ShelfItems = new List<ShelfItem>
                    {
                    }
                },
                new Shelf
                {
                    Id = "5f06c78c7484300017511bae",
                    Name = "Wanted",
                    AccessLevel = AccessLevel.All,
                    OwnerId = "55042217-5552-48bf-88fb-36fef5af23da",
                    ShelfItems = new List<ShelfItem>
                    {
                        new ShelfItem
                        {
                            BookId = 5,
                            BookTitle = "Here?",
                            AuthorName = new List<string> { "Anna" }
                        },
                        new ShelfItem
                        {
                            BookId = 6,
                            BookTitle = "Yes",
                            AuthorName = new List<string> { "Michele" }
                        },
                        new ShelfItem
                        {
                            BookId = 2,
                            BookTitle = "Next title",
                            AuthorName = new List<string> { "Stacy", "Johan" }
                        },
                    }
                }
            };
            ctx.Shelves.InsertMany(shelves);
        }
    }
}
