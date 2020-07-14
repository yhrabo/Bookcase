namespace Bookcase.Services.Catalog.API.Infrastructure
{
    using Catalog.API.Models;
    using EntityConfigurations;
    using Microsoft.EntityFrameworkCore;

    public class CatalogDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }

        public CatalogDbContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AuthorEntityTypeConfiguration());
            builder.ApplyConfiguration(new BookEntityTypeConfiguration());
            builder.ApplyConfiguration(new BookAuthorEntityTypeConfiguration());
        }
    }
}
