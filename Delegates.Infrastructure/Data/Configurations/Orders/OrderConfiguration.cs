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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
            builder.Property(x => x.PaymentMethod).HasMaxLength(50).IsRequired();

            builder.HasIndex(x => new { x.AccountId, x.Code }).IsUnique().HasFilter("[IsDeleted] = 0");

            builder.HasOne(x => x.Client)
                .WithMany()
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.MainWarehouse)
                .WithMany()
                .HasForeignKey(x => x.MainWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.DepositAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.DepositPaymentMethod).HasMaxLength(50);
            builder.Property(x => x.CancellationReason).HasMaxLength(500);
            builder.Property(x => x.DeliveryPostponeReason).HasMaxLength(500);
            builder.Property(x => x.PaymentReceivedMethod).HasMaxLength(50);
        }
    }
}
