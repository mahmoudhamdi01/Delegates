using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Shared
{
    public interface IHasTenant
    {
        int? AccountId { get; set; }
    }
}
