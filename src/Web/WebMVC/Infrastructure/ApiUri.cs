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

            public static string UpdateBook(string baseUri, long id) => $"{baseUri}{id}";
        }

        public static class Shelves
        {
            public static string AddBook(string baseUri, string shelfId)
                   => $"{baseUri}{shelfId}";
            public static string AddShelf(string baseUri) => $"{baseUri}";
            public static string DeleteBook(string baseUri, string shelfId, long bookId)
                   => $"{baseUri}{shelfId}/{bookId}";
            public static string DeleteShelf(string baseUri, string shelfId) => $"{baseUri}{shelfId}";
            public static string GetShelf(string baseUri, string shelfId,
                int pageIndex, int pageSize)
                   => $"{baseUri}{shelfId}?pageIndex={pageIndex}&pageSize={pageSize}";
            public static string GetShelves(string baseUri, string userId, int pageIndex, int pageSize)
                   => $"{baseUri}user/{userId}?pageIndex={pageIndex}&pageSize={pageSize}";
            public static string UpdateShelf(string baseUri, string shelfId) => $"{baseUri}{shelfId}";

        }
    }
}
