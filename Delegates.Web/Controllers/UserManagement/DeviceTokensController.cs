using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.RegisterDevice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delegates.Web.Controllers.UserManagement
{
    [Authorize]
    public class DeviceTokensController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDeviceTokenDto dto)
        {
            await serviceManager.DeviceTokenService.RegisterAsync(dto);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Unregister([FromQuery] string token)
        {
            await serviceManager.DeviceTokenService.UnregisterAsync(token);
            return NoContent();
        }
    }
}
