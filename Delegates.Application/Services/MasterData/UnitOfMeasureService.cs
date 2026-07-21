using AutoMapper;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.MasterData.UnitOfMeasure;
using Delegates.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Services.MasterData
{
    public class UnitOfMeasureService(IUnitOfWork unitOfWork, IEntityAuditHelper auditHelper, IMapper mapper) : IUnitOfMeasureService
    {
        public async Task<UnitOfMeasureReadDto> GetByIdAsync(int id)
        {
            var repo = unitOfWork.GetRepository<UnitOfMeasure, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(UnitOfMeasure), id);
            return mapper.Map<UnitOfMeasureReadDto>(entity);
        }

        public async Task<DataTablePaginationResponseDto<UnitOfMeasureReadDto>> GetPaginationListAsync(DataTablePaginationRequestDto request)
        {
            var repo = unitOfWork.GetRepository<UnitOfMeasure, int>();
            return await repo.GetPaginationListFromBodyAsync(request, x => new UnitOfMeasureReadDto
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

        public async Task<UnitOfMeasureReadDto> CreateAsync(UnitOfMeasureCreateUpdateDto dto)
        {
            var repo = unitOfWork.GetRepository<UnitOfMeasure, int>();
            var entity = mapper.Map<UnitOfMeasure>(dto);
            auditHelper.SetCreated(entity);
            await repo.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<UnitOfMeasureReadDto>(entity);
        }

        public async Task UpdateAsync(int id, UnitOfMeasureCreateUpdateDto dto)
        {
            var repo = unitOfWork.GetRepository<UnitOfMeasure, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(UnitOfMeasure), id);
            mapper.Map(dto, entity);
            auditHelper.SetUpdated(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var repo = unitOfWork.GetRepository<UnitOfMeasure, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(UnitOfMeasure), id);
            auditHelper.SetSoftDeleted(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
