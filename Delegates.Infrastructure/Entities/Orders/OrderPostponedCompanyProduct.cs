using Delegates.Infrastructure.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.Orders
{
    public class OrderPostponedCompanyProduct : BaseEntity<int>
    {
        public int OrderPostponedCompanyId { get; set; }
        public OrderPostponedCompany OrderPostponedCompany { get; set; } = default!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public int Quantity { get; set; }
    }
}
