using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.Company
{
    public class CompanyListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Phone { get; set; } = default!;
    }
}
