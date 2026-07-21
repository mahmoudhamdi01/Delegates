using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.Warehouse
{
    public interface IWarehouseService : ICrudService<int, WarehouseReadDto, WarehouseCreateUpdateDto, WarehouseCreateUpdateDto>
    {
    }
}
