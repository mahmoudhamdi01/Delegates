using Delegates.Infrastructure.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.Orders
{
    public class OrderWarehouseTaskItem : BaseEntity<int>
    {
        public int OrderWarehouseTaskId { get; set; }
        public OrderWarehouseTask OrderWarehouseTask { get; set; } = default!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public int Quantity { get; set; }
    }
}
