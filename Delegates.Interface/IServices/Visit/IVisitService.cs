using Delegates.Infrastructure.Shared.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Visit
{
    public interface IVisitService
    {
        Task<VisitReadDto> CreateAsync(CreateVisitDto dto);
        Task<VisitReadDto> GetByIdAsync(int id);
        Task<VisitReadDto> MarkVisitedAsync(int id, MarkVisitedDto dto);
        Task<DataTablePaginationResponseDto<VisitListDto>> GetMyTodayVisitsAsync(DataTablePaginationRequestDto request);
        Task<DataTablePaginationResponseDto<VisitListDto>> GetMyCreatedVisitsAsync(DataTablePaginationRequestDto request);
        Task<DataTablePaginationResponseDto<AdminVisitListDto>> GetAllVisitsAdminAsync(DataTablePaginationRequestDto request);
    }
}
