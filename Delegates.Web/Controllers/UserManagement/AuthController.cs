using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.UserManagement.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Delegates.Web.Controllers.UserManagement
{
    public class AuthController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpPost("Login")]
        [AllowAnonymous]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        {
            var result = await serviceManager.AuthService.LoginAsync(request);
            return Ok(result);
        }
    }
}
