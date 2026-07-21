using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.MasterData.VisitDestination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delegates.Web.Controllers.MasterData
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class VisitDestinationsController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpPost("GetAll")]
        public async Task<ActionResult<DataTablePaginationResponseDto<VisitDestinationReadDto>>> GetAll(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.VisitDestinationService.GetPaginationListAsync(request));

        [HttpGet("{id}")]
        public async Task<ActionResult<VisitDestinationReadDto>> GetById(int id)
            => Ok(await serviceManager.VisitDestinationService.GetByIdAsync(id));

        [HttpPost]
        public async Task<ActionResult<VisitDestinationReadDto>> Create(VisitDestinationCreateUpdateDto dto)
            => Ok(await serviceManager.VisitDestinationService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, VisitDestinationCreateUpdateDto dto)
        {
            await serviceManager.VisitDestinationService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await serviceManager.VisitDestinationService.DeleteAsync(id);
            return NoContent();
        }
    }
}
