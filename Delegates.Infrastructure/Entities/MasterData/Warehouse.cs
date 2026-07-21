using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.MasterData
{
    public class Warehouse : BaseEntity<int>
    {
        public string Name { get; set; } = default!;
        public string? Note { get; set; }
        public string? Phone { get; set; }
        public string Governorate { get; set; } = default!;
        public string Address { get; set; } = default!;
    }
}
