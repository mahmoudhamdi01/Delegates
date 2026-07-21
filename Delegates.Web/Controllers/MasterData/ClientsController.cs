using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.MasterData.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delegates.Web.Controllers.MasterData
{
    [Authorize(Roles = "Admin,CustomerService")]
    public class ClientsController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpPost("GetAll")]
        public async Task<ActionResult<DataTablePaginationResponseDto<ClientReadDto>>> GetAll(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.ClientService.GetPaginationListAsync(request));

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientReadDto>> GetById(int id)
            => Ok(await serviceManager.ClientService.GetByIdAsync(id));

        [HttpPost]
        public async Task<ActionResult<ClientReadDto>> Create(ClientCreateUpdateDto dto)
            => Ok(await serviceManager.ClientService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ClientCreateUpdateDto dto)
        {
            await serviceManager.ClientService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await serviceManager.ClientService.DeleteAsync(id);
            return NoContent();
        }
    }
}
