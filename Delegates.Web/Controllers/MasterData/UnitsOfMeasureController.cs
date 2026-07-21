using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.MasterData.UnitOfMeasure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delegates.Web.Controllers.MasterData
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UnitsOfMeasureController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpPost("GetAll")]
        public async Task<ActionResult<DataTablePaginationResponseDto<UnitOfMeasureReadDto>>> GetAll(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.UnitOfMeasureService.GetPaginationListAsync(request));

        [HttpGet("{id}")]
        public async Task<ActionResult<UnitOfMeasureReadDto>> GetById(int id)
            => Ok(await serviceManager.UnitOfMeasureService.GetByIdAsync(id));

        [HttpPost]
        public async Task<ActionResult<UnitOfMeasureReadDto>> Create(UnitOfMeasureCreateUpdateDto dto)
            => Ok(await serviceManager.UnitOfMeasureService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UnitOfMeasureCreateUpdateDto dto)
        {
            await serviceManager.UnitOfMeasureService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await serviceManager.UnitOfMeasureService.DeleteAsync(id);
            return NoContent();
        }
    }
}
