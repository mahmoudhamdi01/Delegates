using AutoMapper;
using Delegates.Infrastructure.Entities.Inventory;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Infrastructure.Enums.Inventory;
using Delegates.Infrastructure.Exceptions;
using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.Product;
using Delegates.Interface.IServices.ProductStock;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Services.Inventory
{
    public class ProductService(IUnitOfWork unitOfWork, IEntityAuditHelper auditHelper, IMapper mapper) : IProductService
    {
        private static readonly Expression<Func<Product, ProductReadDto>> Selector = x => new ProductReadDto
        {
            Id = x.Id,
            Name = x.Name,
            CatalogNumber = x.CatalogNumber,
            Price = x.Price,
            Size = x.Size,
            Temperature = x.Temperature,
            ExpiryDate = x.ExpiryDate,
            Notes = x.Notes,
            ProductCategoryId = x.ProductCategoryId,
            CategoryName = x.ProductCategory.Name,
            CompanyId = x.CompanyId,
            CompanyName = x.Company.Name,
            UnitOfMeasureId = x.UnitOfMeasureId,
            UnitOfMeasureName = x.UnitOfMeasure.Name,
            TotalQuantity = x.ProductStocks.Sum(s => s.Quantity),
            ExpiryStatus = x.ExpiryDate == null
                ? ExpiryStatus.Green
                : (x.ExpiryDate.Value.Date < DateTime.UtcNow.Date
                    ? ExpiryStatus.Red
                    : (x.ExpiryDate.Value.Date < DateTime.UtcNow.Date.AddDays(30)
                        ? ExpiryStatus.Yellow
                        : ExpiryStatus.Green)),
            Stocks = x.ProductStocks.Select(s => new ProductStockDto
            {
                WarehouseId = s.WarehouseId,
                WarehouseName = s.Warehouse.Name,
                Quantity = s.Quantity
            }).ToList(),
            CreatedBy = x.CreatedBy,
            CreatedOn = x.CreatedOn,
            LastModifiedBy = x.LastModifiedBy,
            LastModifiedOn = x.LastModifiedOn
        };

        public async Task<ProductReadDto> GetByIdAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Product, int>();
            var result = await repo.TableNoTracking.Where(x => x.Id == id).Select(Selector).FirstOrDefaultAsync();
            return result ?? throw new EntityNotFoundException(nameof(Product), id);
        }

        public Task<DataTablePaginationResponseDto<ProductReadDto>> GetPaginationListAsync(DataTablePaginationRequestDto request)
            => GetPaginationListAsync(request, warehouseId: null);

        public async Task<DataTablePaginationResponseDto<ProductReadDto>> GetPaginationListAsync(DataTablePaginationRequestDto request, int? warehouseId)
        {
            var repo = unitOfWork.GetRepository<Product, int>();

            List<Expression<Func<Product, bool>>>? criterias = warehouseId.HasValue
                ? [x => x.ProductStocks.Any(s => s.WarehouseId == warehouseId.Value)]
                : null;

            return await repo.GetPaginationListFromBodyAsync(request, Selector, criterias: criterias);
        }

        public async Task<ProductReadDto> CreateAsync(ProductCreateUpdateDto dto)
        {
            await ValidateForeignKeysAsync(dto.ProductCategoryId, dto.CompanyId, dto.UnitOfMeasureId);

            var repo = unitOfWork.GetRepository<Product, int>();
            var catalogExists = await repo.TableNoTracking.AnyAsync(x => x.CatalogNumber == dto.CatalogNumber);
            if (catalogExists)
                throw new BadRequestException(["رقم الكتالوج مستخدم بالفعل"]);

            var entity = mapper.Map<Product>(dto);
            auditHelper.SetCreated(entity);
            await repo.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(entity.Id);
        }

        public async Task UpdateAsync(int id, ProductCreateUpdateDto dto)
        {
            await ValidateForeignKeysAsync(dto.ProductCategoryId, dto.CompanyId, dto.UnitOfMeasureId);

            var repo = unitOfWork.GetRepository<Product, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Product), id);

            var catalogExists = await repo.TableNoTracking.AnyAsync(x => x.CatalogNumber == dto.CatalogNumber && x.Id != id);
            if (catalogExists)
                throw new BadRequestException(["رقم الكتالوج مستخدم بالفعل"]);

            mapper.Map(dto, entity);
            auditHelper.SetUpdated(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Product, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Product), id);
            auditHelper.SetSoftDeleted(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<ProductReadDto> SetStockAsync(int productId, SetProductStockDto dto)
        {
            var productsRepo = unitOfWork.GetRepository<Product, int>();
            var productExists = await productsRepo.TableNoTracking.AnyAsync(x => x.Id == productId);
            if (!productExists)
                throw new EntityNotFoundException(nameof(Product), productId);

            var warehouseExists = await unitOfWork.GetRepository<Warehouse, int>().TableNoTracking.AnyAsync(x => x.Id == dto.WarehouseId);
            if (!warehouseExists)
                throw new BadRequestException(["المخزن المحدد غير موجود"]);

            var stockRepo = unitOfWork.GetRepository<ProductStock, int>();
            var stock = await stockRepo.TableNoTracking
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.WarehouseId == dto.WarehouseId);

            if (stock is null)
            {
                stock = new ProductStock { ProductId = productId, WarehouseId = dto.WarehouseId, Quantity = dto.Quantity };
                auditHelper.SetCreated(stock);
                await stockRepo.AddAsync(stock);
            }
            else
            {
                stock.Quantity = dto.Quantity;
                auditHelper.SetUpdated(stock);
                stockRepo.Update(stock);
            }

            await unitOfWork.SaveChangesAsync();
            return await GetByIdAsync(productId);
        }

        private async Task ValidateForeignKeysAsync(int categoryId, int companyId, int unitOfMeasureId)
        {
            var categoryExists = await unitOfWork.GetRepository<ProductCategory, int>().TableNoTracking.AnyAsync(x => x.Id == categoryId);
            if (!categoryExists)
                throw new BadRequestException(["القسم المحدد غير موجود"]);

            var companyExists = await unitOfWork.GetRepository<Company, int>().TableNoTracking.AnyAsync(x => x.Id == companyId);
            if (!companyExists)
                throw new BadRequestException(["الشركة المحددة غير موجودة"]);

            var unitExists = await unitOfWork.GetRepository<UnitOfMeasure, int>().TableNoTracking.AnyAsync(x => x.Id == unitOfMeasureId);
            if (!unitExists)
                throw new BadRequestException(["وحدة القياس المحددة غير موجودة"]);
        }
    }
}
