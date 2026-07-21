using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Shared.Pagination
{
    public class DataTablePaginationResponseDto<TResult>
    {
        public int Draw { get; set; }
        public List<TResult> Data { get; set; } = new();
        public int RecordsTotal { get; set; }      // العدد الكلي قبل أي فلترة/بحث
        public int RecordsFiltered { get; set; }   // العدد بعد الفلترة/البحث
        public int? TotalCount { get; set; }
    }

    public class DataTablePaginationRequestDto
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; } = 10;

        public string? SortColumnName { get; set; }
        public string? SortColumnDirection { get; set; } // "asc" or "desc"

        public string? SearchValue { get; set; }
        public string[]? SearchableCloumns { get; set; }

        public Dictionary<string, string> SearchableCloumnsValues { get; set; } = new();

        public Dictionary<string, string[]> SearchableCloumnsMultiValues { get; set; } = new();

        public Dictionary<string, DateRangeFilterDto> DateRangeFilters { get; set; } = new();
    }

    public class DateRangeFilterDto
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
