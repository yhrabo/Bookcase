using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Infrastructure
{
    // TODO Split between areas.
    public static class ApiUri
    {
        public static class Catalog
        {
            // Author
            public static string AddAuthor(string baseUri) => $"{baseUri}";
            public static string DeleteAuthor(string baseUri, int id) => $"{baseUri}{id}";
            public static string GetAuthor(string baseUri, int id) => $"{baseUri}{id}";
            public static string GetAuthors(string baseUri, int pageIndex, int pageSize)
                => $"{baseUri}?pageIndex={pageIndex}&pageSize={pageSize}";

            public static string UpdateAuthor(string baseUri, int id) => $"{baseUri}{id}";

            // Book
            public static string AddBook(string baseUri) => $"{baseUri}";
            public static string DeleteBook(string baseUri, long id) => $"{baseUri}{id}";
            public static string GetBook(string baseUri, long id) => $"{baseUri}{id}";
            public static string GetBooks(string baseUri, int pageIndex, int pageSize)
                => $"{baseUri}?pageIndex={pageIndex}&pageSize={pageSize}";

            public static string UpdateAuthor(string baseUri, long id) => $"{baseUri}{id}";
        }
    }
}
