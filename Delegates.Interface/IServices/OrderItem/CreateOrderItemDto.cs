using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.OrderItem
{
    public class CreateOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
