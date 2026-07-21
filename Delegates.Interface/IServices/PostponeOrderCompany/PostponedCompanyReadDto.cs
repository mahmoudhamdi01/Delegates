using Delegates.Interface.IServices.PostponeOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.PostponeOrderCompany
{
    public class PostponedCompanyReadDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = default!;
        public List<PostponedProductReadDto> Products { get; set; } = new();
    }
}
