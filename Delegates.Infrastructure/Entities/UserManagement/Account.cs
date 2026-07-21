using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.UserManagement
{
    public class Account : BaseEntity<int>
    {
        public string CompanyName { get; set; } = default!;
    }
}
