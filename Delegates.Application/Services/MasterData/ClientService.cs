using AutoMapper;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Infrastructure.Exceptions;
using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.MasterData.Client;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Services.MasterData
{
    public class ClientService(IUnitOfWork unitOfWork, IEntityAuditHelper auditHelper, IMapper mapper) : IClientService
    {
        public async Task<ClientReadDto> GetByIdAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Client, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Client), id);
            return mapper.Map<ClientReadDto>(entity);
        }

        public async Task<DataTablePaginationResponseDto<ClientReadDto>> GetPaginationListAsync(DataTablePaginationRequestDto request)
        {
            var repo = unitOfWork.GetRepository<Client, int>();
            return await repo.GetPaginationListFromBodyAsync(request, x => new ClientReadDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                PhoneNumber = x.PhoneNumber,
                SecondaryPhoneNumber = x.SecondaryPhoneNumber,
                Governorate = x.Governorate,
                Address = x.Address,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                LastModifiedBy = x.LastModifiedBy,
                LastModifiedOn = x.LastModifiedOn
            });
        }

        public async Task<ClientReadDto> CreateAsync(ClientCreateUpdateDto dto)
        {
            var repo = unitOfWork.GetRepository<Client, int>();

            var phoneExists = await repo.TableNoTracking.AnyAsync(x => x.PhoneNumber == dto.PhoneNumber);
            if (phoneExists)
                throw new BadRequestException(["رقم الهاتف مستخدم بالفعل لعميل آخر"]);

            var entity = mapper.Map<Client>(dto);
            entity.Code = await GenerateClientCodeAsync();

            auditHelper.SetCreated(entity);
            await repo.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();
            return mapper.Map<ClientReadDto>(entity);
        }

        public async Task UpdateAsync(int id, ClientCreateUpdateDto dto)
        {
            var repo = unitOfWork.GetRepository<Client, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Client), id);

            var phoneExists = await repo.TableNoTracking.AnyAsync(x => x.PhoneNumber == dto.PhoneNumber && x.Id != id);
            if (phoneExists)
                throw new BadRequestException(["رقم الهاتف مستخدم بالفعل لعميل آخر"]);

            mapper.Map(dto, entity);
            auditHelper.SetUpdated(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Client, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Client), id);
            auditHelper.SetSoftDeleted(entity);
            repo.Update(entity);
            await unitOfWork.SaveChangesAsync();
        }

        private async Task<string> GenerateClientCodeAsync()
        {
            var repo = unitOfWork.GetRepository<Client, int>();
            var count = await repo.TableNoTrackingWithNoFilter.CountAsync();
            return $"C-{count + 1:D5}";
        }
    }
}
