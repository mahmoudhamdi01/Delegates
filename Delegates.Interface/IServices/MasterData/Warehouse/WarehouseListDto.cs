using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.Warehouse
{
    public class WarehouseListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Governorate { get; set; } = default!;
        public string? Phone { get; set; }
    }
}
