using Delegates.Infrastructure.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Data.Configurations.Orders
{
    public class OrderWarehouseTaskConfiguration : IEntityTypeConfiguration<OrderWarehouseTask>
    {
        public void Configure(EntityTypeBuilder<OrderWarehouseTask> builder)
        {
            builder.Property(x => x.Notes).HasMaxLength(500);

            builder.HasOne(x => x.Order)
                .WithMany(x => x.WarehouseTasks)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Delegate)
                .WithMany()
                .HasForeignKey(x => x.DelegateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
