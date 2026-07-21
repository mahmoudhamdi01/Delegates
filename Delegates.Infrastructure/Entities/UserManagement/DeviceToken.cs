using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.UserManagement
{
    public class DeviceToken : BaseEntity<int>
    {
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = default!;

        public string Token { get; set; } = default!;
        public string? Platform { get; set; }
    }
}
