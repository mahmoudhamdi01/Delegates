using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.CompanyPurchaseProduct
{
    public class CompanyPurchaseRequestDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = default!;
        public List<CompanyPurchaseProductDto> Products { get; set; } = new();
        public List<CompanyPurchaseOrderRefDto> Orders { get; set; } = new();
    }
}
