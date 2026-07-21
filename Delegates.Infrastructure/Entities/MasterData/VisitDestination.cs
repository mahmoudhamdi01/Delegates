using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.MasterData
{
    public class VisitDestination : BaseEntity<int>
    {
        public string Name { get; set; } = default!;
        public string? Note { get; set; }
    }
}
