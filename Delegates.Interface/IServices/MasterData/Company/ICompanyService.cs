using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.Company
{
    public interface ICompanyService : ICrudService<int, CompanyReadDto, CompanyCreateUpdateDto, CompanyCreateUpdateDto>
    {
    }
}
