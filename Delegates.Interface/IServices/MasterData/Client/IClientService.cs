using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.Client
{
    public interface IClientService : ICrudService<int, ClientReadDto, ClientCreateUpdateDto, ClientCreateUpdateDto>
    {
    }
}
