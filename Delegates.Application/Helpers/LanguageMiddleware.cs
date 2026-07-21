using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Helpers
{
    public class LanguageMiddleware
    {
        private readonly RequestDelegate _next;

        public LanguageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var lang = context.Request.Headers["lang"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(lang))
                lang = "en";

            lang = lang.ToLower() switch
            {
                "ar" => "ar",
                _ => "en"
            };

            context.Items["CurrentLanguage"] = lang;

            await _next(context);
        }
    }
}
