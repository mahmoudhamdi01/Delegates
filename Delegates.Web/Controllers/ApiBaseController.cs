using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Delegates.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        protected string GetEmailForToken() => User.FindFirstValue(ClaimTypes.Email)!;
        protected string GetName() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}
