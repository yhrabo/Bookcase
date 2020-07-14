namespace Bookcase.Services.Catalog.API.Infrastructure.EntityConfigurations
{
    using Bookcase.Services.Catalog.API.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    class AuthorEntityTypeConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasIndex(a => a.Name)
                .IsUnique(false);
        }
    }
}
