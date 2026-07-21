using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.CompanyPurchaseProduct;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delegates.Web.Controllers.Orders
{
    [Authorize(Roles = "Admin")]
    public class CompanyPurchasesController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpPost("GetAll")]
        public async Task<ActionResult<DataTablePaginationResponseDto<CompanyPurchaseRequestDto>>> GetAll(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.CompanyPurchaseService.GetPurchaseRequestsAsync(request));
    }
}
