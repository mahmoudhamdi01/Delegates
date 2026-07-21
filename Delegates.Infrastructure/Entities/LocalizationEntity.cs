using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities
{
    public class LocalizationEntity : BaseEntity<int>
    {
        public string TitleArabic { get; set; } = default!;
        public string TitleEnglish { get; set; } = default!;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
    }
}
