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
    public class ProductStockConfiguration : IEntityTypeConfiguration<ProductStock>
    {
        public void Configure(EntityTypeBuilder<ProductStock> builder)
        {
            builder.HasIndex(x => new { x.ProductId, x.WarehouseId }).IsUnique().HasFilter("[IsDeleted] = 0");

            builder.HasOne(x => x.Product)
                .WithMany(x => x.ProductStocks)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
