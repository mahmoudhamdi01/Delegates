using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Visit
{
    public class VisitReadDto
    {
        public int Id { get; set; }
        public int VisitDestinationId { get; set; }
        public string VisitDestinationName { get; set; } = default!;
        public int DelegateId { get; set; }
        public string DelegateName { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string? Notes { get; set; }
        public DateTime? VisitedOn { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

}
