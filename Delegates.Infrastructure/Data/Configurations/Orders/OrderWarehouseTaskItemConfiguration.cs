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
    public class OrderWarehouseTaskItemConfiguration : IEntityTypeConfiguration<OrderWarehouseTaskItem>
    {
        public void Configure(EntityTypeBuilder<OrderWarehouseTaskItem> builder)
        {
            builder.HasOne(x => x.OrderWarehouseTask)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.OrderWarehouseTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
