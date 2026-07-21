using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.UserManagement.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delegates.Web.Controllers.UserManagement
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class UsersController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpPost("GetAll")]
        public async Task<ActionResult<DataTablePaginationResponseDto<UserListItemDto>>> GetAll(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.UserService.GetUsersAsync(request));

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailsDto>> GetDetails(int id)
            => Ok(await serviceManager.UserService.GetUserDetailsAsync(id));

        [HttpPost]
        public async Task<ActionResult<UserDetailsDto>> Create(CreateUserRequestDto request)
            => Ok(await serviceManager.UserService.CreateUserAsync(request));
    }
}
