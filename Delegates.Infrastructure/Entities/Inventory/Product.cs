using Delegates.Infrastructure.Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.Inventory
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; } = default!;
        public string CatalogNumber { get; set; } = default!;
        public decimal Price { get; set; }
        public string? Size { get; set; }
        public string? Temperature { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Notes { get; set; }

        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; } = default!;

        public int CompanyId { get; set; }
        public Company Company { get; set; } = default!;

        public int UnitOfMeasureId { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; } = default!;

        public ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
    }
}
