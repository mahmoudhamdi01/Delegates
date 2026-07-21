using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Product
{
    public class ProductCreateUpdateDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = default!;
        [Required, MaxLength(50)]
        public string CatalogNumber { get; set; } = default!;
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        [MaxLength(50)]
        public string? Size { get; set; }
        [MaxLength(50)]
        public string? Temperature { get; set; }
        public DateTime? ExpiryDate { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public int ProductCategoryId { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int UnitOfMeasureId { get; set; }
    }
}
