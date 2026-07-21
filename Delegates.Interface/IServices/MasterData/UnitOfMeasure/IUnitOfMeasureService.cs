using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.UnitOfMeasure
{
    public interface IUnitOfMeasureService : ICrudService<int, UnitOfMeasureReadDto, UnitOfMeasureCreateUpdateDto, UnitOfMeasureCreateUpdateDto>
    {
    }
}
