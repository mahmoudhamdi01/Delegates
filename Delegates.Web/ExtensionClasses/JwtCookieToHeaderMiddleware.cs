namespace Delegates.Web.ExtensionClasses
{
    public class JwtCookieToHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CookieName = "jwt";

        public JwtCookieToHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip if Authorization header already exists (e.g. Swagger)
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                var token = context.Request.Cookies[CookieName];

                if (!string.IsNullOrEmpty(token))
                    context.Request.Headers.Append("Authorization", $"Bearer {token}");
            }

            await _next(context);
        }
    }
}
