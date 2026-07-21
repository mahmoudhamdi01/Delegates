using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.PostponeOrder
{
    public class PostponedProductReadDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string CatalogNumber { get; set; } = default!;
        public int Quantity { get; set; }
    }

    public class CompletePurchaseItemDto
    {
        [Required]
        public int ProductId { get; set; }
        [Range(1, int.MaxValue)]
        public int ReceivedQuantity { get; set; }
    }

    public class CompletePurchaseDto
    {
        [MinLength(1)]
        public List<CompletePurchaseItemDto> Items { get; set; } = new();
    }
}
