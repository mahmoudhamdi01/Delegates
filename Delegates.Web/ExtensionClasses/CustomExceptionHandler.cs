using Delegates.Infrastructure.Exceptions;
using Delegates.Infrastructure.Exceptions.ErrorModels;

namespace Delegates.Web.ExtensionClasses
{
    public class CustomExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandler> _logger;

        public CustomExceptionHandler(RequestDelegate Next, ILogger<CustomExceptionHandler> logger)
        {
            _next = Next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext _httpContext)
        {
            try
            {
                await _next.Invoke(_httpContext);
                if (_httpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var Response = new ErrorToReturn()
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorMessage = $"End Point {_httpContext.Request.Path} is Not Found"
                    };

                    await _httpContext.Response.WriteAsJsonAsync(Response);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something Went Wrong");

                var isKnownException = ex is NotFoundException or UnAuthorizedException or BadRequestException;

                _httpContext.Response.StatusCode = ex switch
                {
                    NotFoundException => StatusCodes.Status404NotFound,
                    UnAuthorizedException => StatusCodes.Status401Unauthorized,
                    BadRequestException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                var Response = new ErrorToReturn()
                {
                    StatusCode = _httpContext.Response.StatusCode,
                    ErrorMessage = isKnownException ? ex.Message : "حدث خطأ غير متوقع، برجاء المحاولة لاحقًا",
                    Errors = ex is BadRequestException badRequestException ? badRequestException.Errors : null
                };

                await _httpContext.Response.WriteAsJsonAsync(Response);
            }
        }
    }
}
