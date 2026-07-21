using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.Warehouse
{
    public class WarehouseCreateUpdateDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = default!;
        [MaxLength(500)]
        public string? Note { get; set; }
        [MaxLength(20)]
        public string? Phone { get; set; }
        [Required, MaxLength(100)]
        public string Governorate { get; set; } = default!;
        [Required, MaxLength(300)]
        public string Address { get; set; } = default!;
    }
}
