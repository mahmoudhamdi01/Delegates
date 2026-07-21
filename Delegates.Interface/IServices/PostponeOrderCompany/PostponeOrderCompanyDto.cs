using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.PostponeOrderCompany
{
    public class PostponeOrderCompanyDto
    {
        [Required]
        public int CompanyId { get; set; }
        [MinLength(1)]
        public List<int> ProductIds { get; set; } = new();
    }
}
