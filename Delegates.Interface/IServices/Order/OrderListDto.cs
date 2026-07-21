using Delegates.Infrastructure.Enums.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Order
{
    public class OrderListDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string ClientName { get; set; } = default!;
        public string ClientPhoneNumber { get; set; } = default!;
        public OrderStatus Status { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
