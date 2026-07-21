using Delegates.Infrastructure.Shared.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.CompanyPurchaseProduct
{
    public interface ICompanyPurchaseService
    {
        Task<DataTablePaginationResponseDto<CompanyPurchaseRequestDto>> GetPurchaseRequestsAsync(DataTablePaginationRequestDto request);
    }
}
