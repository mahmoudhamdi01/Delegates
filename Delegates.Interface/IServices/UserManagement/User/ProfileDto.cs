using Delegates.Infrastructure.Enums.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.UserManagement.User
{
    public class ProfileDto
    {
        public string FullName { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? SecondaryPhoneNumber { get; set; }
        public string? Address { get; set; }
        public UserType UserType { get; set; }
    }
}
