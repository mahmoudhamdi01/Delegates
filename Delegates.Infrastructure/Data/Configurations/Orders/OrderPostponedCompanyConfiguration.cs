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
    public class OrderPostponedCompanyConfiguration : IEntityTypeConfiguration<OrderPostponedCompany>
    {
        public void Configure(EntityTypeBuilder<OrderPostponedCompany> builder)
        {
            builder.HasOne(x => x.Order)
                .WithMany(x => x.PostponedCompanies)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
