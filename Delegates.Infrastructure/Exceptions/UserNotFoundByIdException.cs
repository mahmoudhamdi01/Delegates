using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Exceptions
{
    public class UserNotFoundByIdException(int id) : NotFoundException($"User With Id {id} Is Not Found")
    {
    }
}
