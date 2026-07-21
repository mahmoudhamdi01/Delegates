using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.Product;
using Delegates.Interface.IServices.ProductStock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delegates.Web.Controllers.Inventory
{
    [Authorize(Roles = "Admin,CustomerService")]
    public class ProductsController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpPost("GetAll")]
        public async Task<ActionResult<DataTablePaginationResponseDto<ProductReadDto>>> GetAll(
            [FromBody] DataTablePaginationRequestDto request, [FromQuery] int? warehouseId)
            => Ok(await serviceManager.ProductService.GetPaginationListAsync(request, warehouseId));

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductReadDto>> GetById(int id)
            => Ok(await serviceManager.ProductService.GetByIdAsync(id));

        [HttpPost]
        public async Task<ActionResult<ProductReadDto>> Create(ProductCreateUpdateDto dto)
            => Ok(await serviceManager.ProductService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCreateUpdateDto dto)
        {
            await serviceManager.ProductService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await serviceManager.ProductService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/Stock")]
        public async Task<ActionResult<ProductReadDto>> SetStock(int id, SetProductStockDto dto)
            => Ok(await serviceManager.ProductService.SetStockAsync(id, dto));
    }
}
