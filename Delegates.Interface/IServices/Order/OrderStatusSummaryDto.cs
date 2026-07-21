using Delegates.Infrastructure.Enums.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Order
{
    public class OrderStatusSummaryDto
    {
        public OrderStatus Status { get; set; }
        public int Count { get; set; }
    }
}
