using Delegates.Infrastructure.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Data.Configurations.Inventory
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
            builder.Property(x => x.CatalogNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Size).HasMaxLength(50);
            builder.Property(x => x.Temperature).HasMaxLength(50);
            builder.Property(x => x.Notes).HasMaxLength(500);

            builder.HasIndex(x => new { x.AccountId, x.CatalogNumber }).IsUnique().HasFilter("[IsDeleted] = 0");

            builder.HasOne(x => x.ProductCategory)
                .WithMany()
                .HasForeignKey(x => x.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UnitOfMeasure)
                .WithMany()
                .HasForeignKey(x => x.UnitOfMeasureId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
