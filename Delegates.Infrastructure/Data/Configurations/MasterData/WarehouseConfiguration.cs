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
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Note).HasMaxLength(500);
            builder.Property(x => x.Phone).HasMaxLength(20);
            builder.Property(x => x.Governorate).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Address).HasMaxLength(300).IsRequired();
        }
    }
}
