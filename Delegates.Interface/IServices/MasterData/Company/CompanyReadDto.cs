using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.Company
{
    public class CompanyReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string? AltPhone { get; set; }
        public string Address { get; set; } = default!;
        public string? Note { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
