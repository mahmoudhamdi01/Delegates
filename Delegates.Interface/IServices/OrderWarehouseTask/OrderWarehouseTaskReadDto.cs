using Delegates.Interface.IServices.OrderItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.OrderWarehouseTask
{
    public class OrderWarehouseTaskReadDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = default!;
        public bool IsMainWarehouse { get; set; }
        public int DelegateId { get; set; }
        public string DelegateName { get; set; } = default!;
        public string? Notes { get; set; }
        public List<OrderItemReadDto> Items { get; set; } = new();
    }
}
