using Delegates.Infrastructure.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.Inventory
{
    public class ProductStock : BaseEntity<int>
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = default!;

        public int Quantity { get; set; }
    }
}
