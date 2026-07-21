using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.Visit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delegates.Web.Controllers.MasterData
{
    [Authorize]
    public class VisitsController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpPost]
        [Authorize(Roles = "CustomerService")]
        public async Task<ActionResult<VisitReadDto>> Create(CreateVisitDto dto)
            => Ok(await serviceManager.VisitService.CreateAsync(dto));

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,CustomerService,Delegate")]
        public async Task<ActionResult<VisitReadDto>> GetById(int id)
            => Ok(await serviceManager.VisitService.GetByIdAsync(id));

        [HttpPost("{id}/MarkVisited")]
        [Authorize(Roles = "Delegate")]
        public async Task<ActionResult<VisitReadDto>> MarkVisited(int id, MarkVisitedDto dto)
            => Ok(await serviceManager.VisitService.MarkVisitedAsync(id, dto));

        [HttpPost("MyToday")]
        [Authorize(Roles = "Delegate")]
        public async Task<ActionResult<DataTablePaginationResponseDto<VisitListDto>>> GetMyToday(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.VisitService.GetMyTodayVisitsAsync(request));

        [HttpPost("MyCreated")]
        [Authorize(Roles = "CustomerService")]
        public async Task<ActionResult<DataTablePaginationResponseDto<VisitListDto>>> GetMyCreated(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.VisitService.GetMyCreatedVisitsAsync(request));

        [HttpPost("AdminAll")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DataTablePaginationResponseDto<AdminVisitListDto>>> GetAllForAdmin(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.VisitService.GetAllVisitsAdminAsync(request));
    }
}
