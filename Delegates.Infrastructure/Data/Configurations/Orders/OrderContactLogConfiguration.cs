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
    public class OrderContactLogConfiguration : IEntityTypeConfiguration<OrderContactLog>
    {
        public void Configure(EntityTypeBuilder<OrderContactLog> builder)
        {
            builder.Property(x => x.Notes).HasMaxLength(500);

            builder.HasOne(x => x.Order)
                .WithMany(x => x.ContactLogs)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
