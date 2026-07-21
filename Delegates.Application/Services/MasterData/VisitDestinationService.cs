using AutoMapper;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.MasterData.VisitDestination;
using Delegates.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Services.MasterData
{
    public class VisitDestinationService(IUnitOfWork unitOfWork, IEntityAuditHelper auditHelper, IMapper mapper) : IVisitDestinationService
    {
        public async Task<VisitDestinationReadDto> GetByIdAsync(int id)
        {
            var repo = unitOfWork.GetRepository<VisitDestination, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(VisitDestination), id);
            return mapper.Map<VisitDestinationReadDto>(entity);
        }

        public async Task<DataTablePaginationResponseDto<VisitDestinationReadDto>> GetPaginationListAsync(DataTablePaginationRequestDto request)
        {
            var repo = unitOfWork.GetRepository<VisitDestination, int>();
            return await repo.GetPaginationListFromBodyAsync(request, x => new VisitDestinationReadDto
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

        public async Task<VisitDestinationReadDto> CreateAsync(VisitDestinationCreateUpdateDto dto)
        {
            var repo = unitOfWork.GetRepository<VisitDestination, int>();
            var entity = mapper.Map<VisitDestination>(dto);
            auditHelper.SetCreated(entity);
            await repo.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<VisitDestinationReadDto>(entity);
        }

        public async Task UpdateAsync(int id, VisitDestinationCreateUpdateDto dto)
        {
            var repo = unitOfWork.GetRepository<VisitDestination, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(VisitDestination), id);
            mapper.Map(dto, entity);
            auditHelper.SetUpdated(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var repo = unitOfWork.GetRepository<VisitDestination, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(VisitDestination), id);
            auditHelper.SetSoftDeleted(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
