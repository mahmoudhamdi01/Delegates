using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Order
{
    public class DeliverOrderDto
    {
        [Required, MaxLength(50)]
        public string PaymentReceivedMethod { get; set; } = default!;
    }

    public class CancelOrderDto
    {
        [Required, MaxLength(500)]
        public string Reason { get; set; } = default!;
    }

    public class PostponeDeliveryDto
    {
        [Required, MaxLength(500)]
        public string Reason { get; set; } = default!;
    }

    public class ReassignDelegateDto
    {
        [Required]
        public int NewDelegateId { get; set; }
    }

    public class CancelledOrderListDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string ClientName { get; set; } = default!;
        public string DelegateName { get; set; } = default!;
        public string CancellationReason { get; set; } = default!;
        public DateTime? CancelledOn { get; set; }
    }

    public class DeliveryPostponedOrderListDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string ClientName { get; set; } = default!;
        public string DelegateName { get; set; } = default!;
        public string PostponeReason { get; set; } = default!;
        public DateTime? PostponedOn { get; set; }
    }
}
