using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Infrastructure.Entities.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.Orders
{
    public class OrderWarehouseTask : BaseEntity<int>
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = default!;

        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = default!;

        public int DelegateId { get; set; }
        public ApplicationUser Delegate { get; set; } = default!;

        public bool IsMainWarehouse { get; set; }
        public string? Notes { get; set; }

        public ICollection<OrderWarehouseTaskItem> Items { get; set; } = new List<OrderWarehouseTaskItem>();
    }
}
