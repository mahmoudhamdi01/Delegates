using Delegates.Interface.IServices.OrderWarehouseTask;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Order
{
    public class CreateOrderDto
    {
        [Required]
        public int ClientId { get; set; }
        [Required, MaxLength(50)]
        public string PaymentMethod { get; set; } = default!;
        [Required]
        public CreateOrderWarehouseTaskDto MainWarehouseTask { get; set; } = default!;
        public List<CreateOrderWarehouseTaskDto> SubWarehouseTasks { get; set; } = new();
    }
}
