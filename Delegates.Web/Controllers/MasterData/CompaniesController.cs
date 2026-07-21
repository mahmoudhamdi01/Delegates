using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.MasterData.Company;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delegates.Web.Controllers.MasterData
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CompaniesController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpPost("GetAll")]
        public async Task<ActionResult<DataTablePaginationResponseDto<CompanyReadDto>>> GetAll(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.CompanyService.GetPaginationListAsync(request));

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyReadDto>> GetById(int id)
            => Ok(await serviceManager.CompanyService.GetByIdAsync(id));

        [HttpPost]
        public async Task<ActionResult<CompanyReadDto>> Create(CompanyCreateUpdateDto dto)
            => Ok(await serviceManager.CompanyService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CompanyCreateUpdateDto dto)
        {
            await serviceManager.CompanyService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await serviceManager.CompanyService.DeleteAsync(id);
            return NoContent();
        }
    }
}
