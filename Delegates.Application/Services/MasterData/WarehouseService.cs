using AutoMapper;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.MasterData.Warehouse;
using Delegates.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Services.MasterData
{
    public class WarehouseService(IUnitOfWork unitOfWork, IEntityAuditHelper auditHelper, IMapper mapper) : IWarehouseService
    {
        public async Task<WarehouseReadDto> GetByIdAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Warehouse, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Warehouse), id);
            return mapper.Map<WarehouseReadDto>(entity);
        }

        public async Task<DataTablePaginationResponseDto<WarehouseReadDto>> GetPaginationListAsync(DataTablePaginationRequestDto request)
        {
            var repo = unitOfWork.GetRepository<Warehouse, int>();
            return await repo.GetPaginationListFromBodyAsync(request, x => new WarehouseReadDto
            {
                Id = x.Id,
                Name = x.Name,
                Note = x.Note,
                Phone = x.Phone,
                Governorate = x.Governorate,
                Address = x.Address,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                LastModifiedBy = x.LastModifiedBy,
                LastModifiedOn = x.LastModifiedOn
            });
        }

        public async Task<WarehouseReadDto> CreateAsync(WarehouseCreateUpdateDto dto)
        {
            var repo = unitOfWork.GetRepository<Warehouse, int>();
            var entity = mapper.Map<Warehouse>(dto);
            auditHelper.SetCreated(entity);
            await repo.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<WarehouseReadDto>(entity);
        }

        public async Task UpdateAsync(int id, WarehouseCreateUpdateDto dto)
        {
            var repo = unitOfWork.GetRepository<Warehouse, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Warehouse), id);
            mapper.Map(dto, entity);
            auditHelper.SetUpdated(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Warehouse, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Warehouse), id);
            auditHelper.SetSoftDeleted(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
