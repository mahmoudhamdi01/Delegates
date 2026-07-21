using Delegates.Interface.IServices.OrderItem;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.OrderWarehouseTask
{
    public class CreateOrderWarehouseTaskDto
    {
        [Required]
        public int WarehouseId { get; set; }
        [Required]
        public int DelegateId { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
        [MinLength(1)]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
