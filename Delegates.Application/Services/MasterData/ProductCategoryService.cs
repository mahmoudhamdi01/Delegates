using AutoMapper;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.MasterData.ProductCategory;
using Delegates.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Services.MasterData
{
    public class ProductCategoryService(IUnitOfWork unitOfWork, IEntityAuditHelper auditHelper, IMapper mapper) : IProductCategoryService
    {
        public async Task<ProductCategoryReadDto> GetByIdAsync(int id)
        {
            var repo = unitOfWork.GetRepository<ProductCategory, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(ProductCategory), id);
            return mapper.Map<ProductCategoryReadDto>(entity);
        }

        public async Task<DataTablePaginationResponseDto<ProductCategoryReadDto>> GetPaginationListAsync(DataTablePaginationRequestDto request)
        {
            var repo = unitOfWork.GetRepository<ProductCategory, int>();
            return await repo.GetPaginationListFromBodyAsync(request, x => new ProductCategoryReadDto
            {
                Id = x.Id,
                Name = x.Name,
                Note = x.Note,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                LastModifiedBy = x.LastModifiedBy,
                LastModifiedOn = x.LastModifiedOn
            });
        }

        public async Task<ProductCategoryReadDto> CreateAsync(ProductCategoryCreateUpdateDto dto)
        {
            var repo = unitOfWork.GetRepository<ProductCategory, int>();
            var entity = mapper.Map<ProductCategory>(dto);
            auditHelper.SetCreated(entity);
            await repo.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<ProductCategoryReadDto>(entity);
        }

        public async Task UpdateAsync(int id, ProductCategoryCreateUpdateDto dto)
        {
            var repo = unitOfWork.GetRepository<ProductCategory, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(ProductCategory), id);
            mapper.Map(dto, entity);
            auditHelper.SetUpdated(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var repo = unitOfWork.GetRepository<ProductCategory, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(ProductCategory), id);
            auditHelper.SetSoftDeleted(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
