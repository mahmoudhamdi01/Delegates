using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.IServices.ProductStock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Product
{
    public interface IProductService : ICrudService<int, ProductReadDto, ProductCreateUpdateDto, ProductCreateUpdateDto>
    {
        Task<DataTablePaginationResponseDto<ProductReadDto>> GetPaginationListAsync(DataTablePaginationRequestDto request, int? warehouseId);
        Task<ProductReadDto> SetStockAsync(int productId, SetProductStockDto dto);
    }
}
