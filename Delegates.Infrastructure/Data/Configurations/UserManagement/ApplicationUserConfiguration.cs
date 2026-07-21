using Delegates.Infrastructure.Entities.UserManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Data.Configurations.UserManagement
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            builder.Property(x => x.PhoneNumber).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
            builder.Property(x => x.PasswordHash).IsRequired();

            builder.HasIndex(x => x.PhoneNumber).IsUnique().HasFilter("[IsDeleted] = 0");
            builder.HasIndex(x => x.Code).IsUnique().HasFilter("[IsDeleted] = 0");
        }
    }
}
