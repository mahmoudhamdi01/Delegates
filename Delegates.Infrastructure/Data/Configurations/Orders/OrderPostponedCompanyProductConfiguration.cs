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
    public class OrderPostponedCompanyProductConfiguration : IEntityTypeConfiguration<OrderPostponedCompanyProduct>
    {
        public void Configure(EntityTypeBuilder<OrderPostponedCompanyProduct> builder)
        {
            builder.HasOne(x => x.OrderPostponedCompany)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.OrderPostponedCompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
