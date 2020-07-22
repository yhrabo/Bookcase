namespace Bookcase.Services.Catalog.API.Infrastructure.EntityConfigurations
{
    using Bookcase.Services.Catalog.API.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasIndex(b => b.Title)
                .IsUnique(false);

            builder.HasIndex(b => b.ISBN)
                .IsUnique();

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(Book.TitleMaxLength);
        }
    }
}
