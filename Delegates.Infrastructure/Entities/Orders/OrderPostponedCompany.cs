using Delegates.Infrastructure.Entities.Inventory;
using Delegates.Infrastructure.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.Orders
{
    public class OrderPostponedCompany : BaseEntity<int>
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = default!;

        public int CompanyId { get; set; }
        public Company Company { get; set; } = default!;

        public ICollection<OrderPostponedCompanyProduct> Products { get; set; } = new List<OrderPostponedCompanyProduct>();
    }
}
