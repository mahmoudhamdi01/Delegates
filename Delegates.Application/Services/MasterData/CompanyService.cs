using AutoMapper;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Infrastructure.Exceptions;
using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.MasterData.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Services.MasterData
{
    public class CompanyService(IUnitOfWork unitOfWork, IEntityAuditHelper auditHelper, IMapper mapper) : ICompanyService
    {
        public async Task<CompanyReadDto> GetByIdAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Company, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Company), id);
            return mapper.Map<CompanyReadDto>(entity);
        }

        public async Task<DataTablePaginationResponseDto<CompanyReadDto>> GetPaginationListAsync(DataTablePaginationRequestDto request)
        {
            var repo = unitOfWork.GetRepository<Company, int>();
            return await repo.GetPaginationListFromBodyAsync(request, x => new CompanyReadDto
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                AltPhone = x.AltPhone,
                Address = x.Address,
                Note = x.Note,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                LastModifiedBy = x.LastModifiedBy,
                LastModifiedOn = x.LastModifiedOn
            });
        }

        public async Task<CompanyReadDto> CreateAsync(CompanyCreateUpdateDto dto)
        {
            var repo = unitOfWork.GetRepository<Company, int>();
            var entity = mapper.Map<Company>(dto);
            auditHelper.SetCreated(entity);
            await repo.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<CompanyReadDto>(entity);
        }

        public async Task UpdateAsync(int id, CompanyCreateUpdateDto dto)
        {
            var repo = unitOfWork.GetRepository<Company, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Company), id);
            mapper.Map(dto, entity);
            auditHelper.SetUpdated(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Company, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Company), id);
            auditHelper.SetSoftDeleted(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
