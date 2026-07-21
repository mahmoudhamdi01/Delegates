using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.Orders
{
    public class OrderContactLog : BaseEntity<int>
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = default!;
        public string? Notes { get; set; }
    }
}
