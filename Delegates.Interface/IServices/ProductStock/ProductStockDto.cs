using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.ProductStock
{
    public class ProductStockDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = default!;
        public int Quantity { get; set; }
    }
}
