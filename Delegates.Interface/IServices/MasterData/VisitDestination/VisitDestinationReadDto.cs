using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.VisitDestination
{
    public class VisitDestinationReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Note { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
