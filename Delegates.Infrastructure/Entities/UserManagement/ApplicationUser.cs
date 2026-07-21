using Delegates.Infrastructure.Enums.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.UserManagement
{
    public class ApplicationUser : BaseEntity<int>
    {
        public string FullName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;      // Unique 
        public string? SecondaryPhoneNumber { get; set; }
        public string PasswordHash { get; set; } = default!;
        public string Code { get; set; } = default!;              // Unique
        public string? Address { get; set; }
        public UserType UserType { get; set; }
    }
}
