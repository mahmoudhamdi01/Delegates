using Delegates.Infrastructure.Shared.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices
{
    public interface ICrudService<TKey, TReadDto, TCreateDto, TUpdateDto>
        where TKey : IEquatable<TKey>
        where TReadDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        Task<TReadDto> GetByIdAsync(TKey id);
        Task<DataTablePaginationResponseDto<TReadDto>> GetPaginationListAsync(DataTablePaginationRequestDto request);
        Task<TReadDto> CreateAsync(TCreateDto dto);
        Task UpdateAsync(TKey id, TUpdateDto dto);
        Task DeleteAsync(TKey id);
    }
}
