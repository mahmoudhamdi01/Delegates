using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.MasterData.ProductCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Delegates.Web.Controllers.MasterData
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ProductCategoriesController(IServiceManager serviceManager) : ApiBaseController
    {
        [HttpPost("GetAll")]
        public async Task<ActionResult<DataTablePaginationResponseDto<ProductCategoryReadDto>>> GetAll(DataTablePaginationRequestDto request)
            => Ok(await serviceManager.ProductCategoryService.GetPaginationListAsync(request));

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategoryReadDto>> GetById(int id)
            => Ok(await serviceManager.ProductCategoryService.GetByIdAsync(id));

        [HttpPost]
        public async Task<ActionResult<ProductCategoryReadDto>> Create(ProductCategoryCreateUpdateDto dto)
            => Ok(await serviceManager.ProductCategoryService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCategoryCreateUpdateDto dto)
        {
            await serviceManager.ProductCategoryService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await serviceManager.ProductCategoryService.DeleteAsync(id);
            return NoContent();
        }
    }
}
