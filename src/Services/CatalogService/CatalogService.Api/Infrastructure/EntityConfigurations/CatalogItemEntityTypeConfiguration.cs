using CatalogService.Api.Core.Domain.Enitites;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Infrastructure.EntityConfigurations
{
    public class CatalogItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("Catalog", CatalogContext.DEFAULT_SCHEMA);


            builder.Property(p => p.Id).UseHiLo("catalog_hilo").IsRequired();

            builder.Property(p => p.Name).IsRequired().HasMaxLength(128);

            builder.Property(p => p.Price).IsRequired();

            builder.Property(p => p.PictureFileName).IsRequired(false);

            builder.Ignore(p => p.PictureUri);

            builder.HasOne(p => p.CatalogBrand).WithMany().HasForeignKey(p => p.CatalogBrandId);

            builder.HasOne(p => p.CatalogType).WithMany().HasForeignKey(p => p.CatalogTypeId);

        }
    }
}
