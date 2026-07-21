using Delegates.Infrastructure.Enums.Orders;
using Delegates.Interface.IServices.OrderStatusHistory;
using Delegates.Interface.IServices.OrderWarehouseTask;
using Delegates.Interface.IServices.PostponeOrderCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Order
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public int ClientId { get; set; }
        public string ClientName { get; set; } = default!;
        public string ClientPhoneNumber { get; set; } = default!;
        public int MainWarehouseId { get; set; }
        public string MainWarehouseName { get; set; } = default!;
        public string PaymentMethod { get; set; } = default!;
        public OrderStatus Status { get; set; }
        public List<OrderWarehouseTaskReadDto> WarehouseTasks { get; set; } = new();
        public List<OrderStatusHistoryReadDto> StatusHistory { get; set; } = new();
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public decimal? DepositAmount { get; set; }
        public string? DepositPaymentMethod { get; set; }
        public string? CancellationReason { get; set; }
        public string? DeliveryPostponeReason { get; set; }
        public string? PaymentReceivedMethod { get; set; }
        public List<PostponedCompanyReadDto> PostponedCompanies { get; set; } = new();
        public List<OrderContactLogReadDto> ContactLogs { get; set; } = new();
    }
}
