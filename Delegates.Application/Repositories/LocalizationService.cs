using Delegates.Infrastructure.Entities;
using Delegates.Interface.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Repositories
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalizationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentLanguage()
        {
            return _httpContextAccessor.HttpContext?.Items["CurrentLanguage"]?.ToString() ?? "en";
        }

        public string GetLocalizedTitle(LocalizationEntity entity)
        {
            if (entity is null)
                return string.Empty;

            var lang = GetCurrentLanguage().ToLower();

            return lang switch
            {
                "ar" => entity.TitleArabic ?? entity.TitleEnglish ?? string.Empty,
                "en" => entity.TitleEnglish ?? entity.TitleArabic ?? string.Empty,
                _ => entity.TitleArabic ?? entity.TitleEnglish ?? string.Empty
            };
        }

        public string GetLocalizedDescription(LocalizationEntity entity)
        {
            if (entity is null)
                return string.Empty;

            var lang = GetCurrentLanguage().ToLower();

            return lang switch
            {
                "ar" => entity.DescriptionAr ?? entity.DescriptionEn ?? string.Empty,
                "en" => entity.DescriptionEn ?? entity.DescriptionAr ?? string.Empty,
                _ => entity.DescriptionAr ?? entity.DescriptionEn ?? string.Empty
            };
        }

        public List<string> GetLocalizedList(List<string>? ar, List<string>? en)
        {
            var lang = GetCurrentLanguage().ToLower();

            return lang switch
            {
                "ar" => ar ?? en ?? new List<string>(),
                "en" => en ?? ar ?? new List<string>(),
                _ => ar ?? en ?? new List<string>()
            };
        }
    }
}
