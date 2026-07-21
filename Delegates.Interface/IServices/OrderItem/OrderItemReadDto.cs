using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.OrderItem
{
    public class OrderItemReadDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }
    }

}
