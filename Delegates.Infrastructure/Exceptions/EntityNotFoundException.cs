using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Exceptions
{
    public class EntityNotFoundException(string entityName, int id)
    : NotFoundException($"{entityName} With Id {id} Is Not Found")
    {
    }
}
