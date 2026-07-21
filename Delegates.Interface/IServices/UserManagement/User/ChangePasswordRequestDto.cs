using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.UserManagement.User
{
    public class ChangePasswordRequestDto
    {
        public string CurrentPassword { get; set; } = default!;
        [MinLength(8)]
        public string NewPassword { get; set; } = default!;
    }
}
