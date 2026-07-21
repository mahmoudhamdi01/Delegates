using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Helpers
{
    public static class LanguageMiddlewareExtensions
    {
        public static IApplicationBuilder UseLanguageMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LanguageMiddleware>();
        }
    }
}
