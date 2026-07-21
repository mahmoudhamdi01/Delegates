using Delegates.Infrastructure.Enums.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.OrderStatusHistory
{
    public class OrderStatusHistoryReadDto
    {
        public OrderStatus Status { get; set; }
        public string? ChangedBy { get; set; }
        public DateTime? ChangedOn { get; set; }
    }
}
