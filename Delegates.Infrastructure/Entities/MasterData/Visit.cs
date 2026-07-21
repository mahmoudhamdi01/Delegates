using Delegates.Infrastructure.Entities.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities.MasterData
{
    public class Visit : BaseEntity<int>
    {
        public int VisitDestinationId { get; set; }
        public VisitDestination VisitDestination { get; set; } = default!;

        public int DelegateId { get; set; }
        public ApplicationUser Delegate { get; set; } = default!;

        public string Address { get; set; } = default!;
        public string? Notes { get; set; }

        public DateTime? VisitedOn { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
