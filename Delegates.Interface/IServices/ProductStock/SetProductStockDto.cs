using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.ProductStock
{
    public class SetProductStockDto
    {
        [Required]
        public int WarehouseId { get; set; }
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
