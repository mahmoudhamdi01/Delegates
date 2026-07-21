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
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Note).HasMaxLength(500);
        }
    }
}
