using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Infrastructure.Enums.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.Orders
{
    public class Order : BaseEntity<int>
    {
        public string Code { get; set; } = default!;

        public int ClientId { get; set; }
        public Client Client { get; set; } = default!;

        public int MainWarehouseId { get; set; }
        public Warehouse MainWarehouse { get; set; } = default!;

        public string PaymentMethod { get; set; } = default!;
        public OrderStatus Status { get; set; }

        public decimal? DepositAmount { get; set; }
        public string? DepositPaymentMethod { get; set; }
        public string? CancellationReason { get; set; }
        public string? DeliveryPostponeReason { get; set; }
        public string? PaymentReceivedMethod { get; set; }
        public ICollection<OrderWarehouseTask> WarehouseTasks { get; set; } = new List<OrderWarehouseTask>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
        //public ICollection<OrderPostponedCompany> PostponedCompanies { get; set; } = new List<OrderPostponedCompany>();
        public ICollection<OrderPostponedCompany> PostponedCompanies { get; set; } = new List<OrderPostponedCompany>();
        public ICollection<OrderContactLog> ContactLogs { get; set; } = new List<OrderContactLog>();
    }
}
