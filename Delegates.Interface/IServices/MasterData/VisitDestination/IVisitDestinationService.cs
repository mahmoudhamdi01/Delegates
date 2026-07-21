using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.VisitDestination
{
    public interface IVisitDestinationService : ICrudService<int, VisitDestinationReadDto, VisitDestinationCreateUpdateDto, VisitDestinationCreateUpdateDto>
    {
    }
}
