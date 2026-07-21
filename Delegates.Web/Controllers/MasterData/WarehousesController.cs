using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.MasterData.Warehouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delegates.Web.Controllers.MasterData
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class WarehousesController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpPost("GetAll")]
        public async Task<ActionResult<DataTablePaginationResponseDto<WarehouseReadDto>>> GetAll(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.WarehouseService.GetPaginationListAsync(request));

        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseReadDto>> GetById(int id)
            => Ok(await serviceManager.WarehouseService.GetByIdAsync(id));

        [HttpPost]
        public async Task<ActionResult<WarehouseReadDto>> Create(WarehouseCreateUpdateDto dto)
            => Ok(await serviceManager.WarehouseService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, WarehouseCreateUpdateDto dto)
        {
            await serviceManager.WarehouseService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await serviceManager.WarehouseService.DeleteAsync(id);
            return NoContent();
        }
    }
}
