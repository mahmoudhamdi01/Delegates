using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.UserManagement.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delegates.Web.Controllers.UserManagement
{
    [Authorize]
    public class ProfileController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpGet]
        public async Task<ActionResult<ProfileDto>> GetProfile()
            => Ok(await serviceManager.UserService.GetProfileAsync());

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request)
        {
            await serviceManager.UserService.ChangePasswordAsync(request);
            return NoContent();
        }

        [HttpPut("SecondaryPhone")]
        public async Task<IActionResult> UpdateSecondaryPhone(UpdateSecondaryPhoneRequestDto request)
        {
            await serviceManager.UserService.UpdateSecondaryPhoneAsync(request);
            return NoContent();
        }
    }
}
