using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Enums.Orders
{
    public enum OrderStatus
    {
        Inquiry = 1,
        Approved = 2,
        Postponed = 3,
        Cancelled = 4,
        Delivered = 5,
        DeliveryPostponed = 6
    }
}
