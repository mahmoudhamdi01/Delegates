using Delegates.Infrastructure.Entities.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Data.Configurations.MasterData
{
    public class VisitConfiguration : IEntityTypeConfiguration<Visit>
    {
        public void Configure(EntityTypeBuilder<Visit> builder)
        {
            builder.Property(x => x.Address).HasMaxLength(300).IsRequired();
            builder.Property(x => x.Notes).HasMaxLength(500);
            builder.Property(x => x.Latitude).HasColumnType("float");
            builder.Property(x => x.Longitude).HasColumnType("float");

            builder.HasOne(x => x.VisitDestination)
                .WithMany()
                .HasForeignKey(x => x.VisitDestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Delegate)
                .WithMany()
                .HasForeignKey(x => x.DelegateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
