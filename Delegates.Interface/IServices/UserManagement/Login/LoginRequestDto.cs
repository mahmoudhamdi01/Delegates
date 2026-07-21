using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.UserManagement.Login
{
    public class LoginRequestDto
    {
        public string PhoneNumber { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
