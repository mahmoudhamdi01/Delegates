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
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Phone).HasMaxLength(20).IsRequired();
            builder.Property(x => x.AltPhone).HasMaxLength(20);
            builder.Property(x => x.Address).HasMaxLength(300).IsRequired();
            builder.Property(x => x.Note).HasMaxLength(500);
        }
    }
}
