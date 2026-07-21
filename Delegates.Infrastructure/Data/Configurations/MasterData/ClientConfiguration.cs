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
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
            builder.Property(x => x.PhoneNumber).HasMaxLength(20).IsRequired();
            builder.Property(x => x.SecondaryPhoneNumber).HasMaxLength(20);
            builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Governorate).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Address).HasMaxLength(300).IsRequired();

            builder.HasIndex(x => new { x.AccountId, x.PhoneNumber }).IsUnique().HasFilter("[IsDeleted] = 0");
            builder.HasIndex(x => new { x.AccountId, x.Code }).IsUnique().HasFilter("[IsDeleted] = 0");
        }
    }
}
