using Delegates.Infrastructure.Enums.Inventory;
using Delegates.Interface.IServices.ProductStock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Product
{
    public class ProductReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string CatalogNumber { get; set; } = default!;
        public decimal Price { get; set; }
        public string? Size { get; set; }
        public string? Temperature { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Notes { get; set; }

        public int ProductCategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = default!;
        public int UnitOfMeasureId { get; set; }
        public string UnitOfMeasureName { get; set; } = default!;

        public int TotalQuantity { get; set; }
        public ExpiryStatus ExpiryStatus { get; set; }
        public List<ProductStockDto> Stocks { get; set; } = new();

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
