using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.MasterData
{
    public class Client : BaseEntity<int>
    {
        public string Name { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? SecondaryPhoneNumber { get; set; }
        public string Code { get; set; } = default!;
        public string Governorate { get; set; } = default!;
        public string Address { get; set; } = default!;
    }
}
