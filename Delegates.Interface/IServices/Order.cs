using Delegates.Interface.IServices.OrderItem;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices
{
    public class CreateOrderDto
    {
        [Required]
        public int ClientId { get; set; }
        [Required]
        public int MainWarehouseId { get; set; }
        [Required]
        public int DelegateId { get; set; }
        [Required, MaxLength(50)]
        public string PaymentMethod { get; set; } = default!;
        [MaxLength(500)]
        public string? Notes { get; set; }
        [MinLength(1)]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
