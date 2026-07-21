using Delegates.Infrastructure.Enums.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.UserManagement.User
{
    public class UserListItemDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public UserType UserType { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedById { get; set; }
        public string? CreatedByName { get; set; }
    }
}
