using Delegates.Infrastructure.Entities.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(ApplicationUser user);
    }
}
