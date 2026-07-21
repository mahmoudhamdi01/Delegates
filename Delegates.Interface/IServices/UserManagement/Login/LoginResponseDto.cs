using Delegates.Infrastructure.Enums.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.UserManagement.Login
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Code { get; set; } = default!;
        public UserType UserType { get; set; }
    }
}
