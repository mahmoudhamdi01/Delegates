using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.CompanyPurchaseProduct
{
    public class CompanyPurchaseProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string CatalogNumber { get; set; } = default!;
        public int TotalQuantityNeeded { get; set; }
    }
}
