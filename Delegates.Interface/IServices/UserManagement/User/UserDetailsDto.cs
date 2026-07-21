using Delegates.Infrastructure.Enums.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.UserManagement.User
{
    public class UserDetailsDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? SecondaryPhoneNumber { get; set; }
        public string? Address { get; set; }
        public UserType UserType { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedById { get; set; }
        public string? CreatedByName { get; set; }
    }
}
