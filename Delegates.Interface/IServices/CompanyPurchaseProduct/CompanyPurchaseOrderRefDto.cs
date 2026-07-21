using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.CompanyPurchaseProduct
{
    public class CompanyPurchaseOrderRefDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = default!;
    }
}
