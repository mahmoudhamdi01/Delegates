using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Visit
{
    public class AdminVisitListDto
    {
        public int Id { get; set; }
        public string VisitDestinationName { get; set; } = default!;
        public string DelegateName { get; set; } = default!;
        public bool IsVisited { get; set; }
        public DateTime? VisitedOn { get; set; }
        public string? CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
