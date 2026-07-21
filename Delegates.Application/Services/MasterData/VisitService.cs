using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Infrastructure.Entities.UserManagement;
using Delegates.Infrastructure.Enums.UserManagement;
using Delegates.Infrastructure.Exceptions;
using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.Visit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Services.MasterData
{
    public class VisitService(IUnitOfWork unitOfWork, IEntityAuditHelper auditHelper) : IVisitService
    {
        private static readonly Expression<Func<Visit, VisitListDto>> ListSelector = x => new VisitListDto
        {
            Id = x.Id,
            VisitDestinationName = x.VisitDestination.Name,
            DelegateName = x.Delegate.FullName,
            Address = x.Address,
            IsVisited = x.VisitedOn != null,
            VisitedOn = x.VisitedOn,
            CreatedOn = x.CreatedOn
        };

        public async Task<VisitReadDto> CreateAsync(CreateVisitDto dto)
        {
            var destinationExists = await unitOfWork.GetRepository<VisitDestination, int>().TableNoTracking.AnyAsync(x => x.Id == dto.VisitDestinationId);
            if (!destinationExists)
                throw new BadRequestException(["جهة الزيارة المحددة غير موجودة"]);

            var delegateUser = await unitOfWork.GetRepository<ApplicationUser, int>().TableNoTracking
                .FirstOrDefaultAsync(x => x.Id == dto.DelegateId);
            if (delegateUser is null || delegateUser.UserType != UserType.Delegate)
                throw new BadRequestException(["المندوب المحدد غير موجود"]);

            var visitsRepo = unitOfWork.GetRepository<Visit, int>();
            var today = DateTime.UtcNow.Date;
            var hasVisitToday = await visitsRepo.TableNoTracking
                .AnyAsync(x => x.DelegateId == dto.DelegateId && x.CreatedOn != null && x.CreatedOn.Value.Date == today);

            if (hasVisitToday)
                throw new BadRequestException(["هذا المندوب لديه بالفعل زيارة مجدولة اليوم"]);

            var visit = new Visit
            {
                VisitDestinationId = dto.VisitDestinationId,
                DelegateId = dto.DelegateId,
                Address = dto.Address,
                Notes = dto.Notes
            };
            auditHelper.SetCreated(visit);

            await visitsRepo.AddAsync(visit);
            await unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(visit.Id);
        }

        public async Task<VisitReadDto> GetByIdAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Visit, int>();
            var visit = await repo.TableNoTracking
                .Include(x => x.VisitDestination)
                .Include(x => x.Delegate)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new EntityNotFoundException(nameof(Visit), id);

            return new VisitReadDto
            {
                Id = visit.Id,
                VisitDestinationId = visit.VisitDestinationId,
                VisitDestinationName = visit.VisitDestination.Name,
                DelegateId = visit.DelegateId,
                DelegateName = visit.Delegate.FullName,
                Address = visit.Address,
                Notes = visit.Notes,
                VisitedOn = visit.VisitedOn,
                Latitude = visit.Latitude,
                Longitude = visit.Longitude,
                CreatedBy = visit.CreatedBy,
                CreatedOn = visit.CreatedOn
            };
        }

        public async Task<VisitReadDto> MarkVisitedAsync(int id, MarkVisitedDto dto)
        {
            var repo = unitOfWork.GetRepository<Visit, int>();
            var visit = await repo.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Visit), id);

            var currentUserId = int.Parse(auditHelper.GetCurrentUserId()!);
            if (visit.DelegateId != currentUserId)
                throw new BadRequestException(["هذه الزيارة غير مسندة إليك"]);

            if (visit.VisitedOn != null)
                throw new BadRequestException(["تم تسجيل هذه الزيارة كمكتملة بالفعل"]);

            visit.VisitedOn = DateTime.UtcNow;
            visit.Latitude = dto.Latitude;
            visit.Longitude = dto.Longitude;
            auditHelper.SetUpdated(visit);

            repo.Update(visit);
            await unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<DataTablePaginationResponseDto<VisitListDto>> GetMyTodayVisitsAsync(DataTablePaginationRequestDto request)
        {
            var currentUserId = int.Parse(auditHelper.GetCurrentUserId()!);
            var today = DateTime.UtcNow.Date;
            var repo = unitOfWork.GetRepository<Visit, int>();

            List<Expression<Func<Visit, bool>>> criterias =
                [x => x.DelegateId == currentUserId && x.CreatedOn != null && x.CreatedOn.Value.Date == today];

            return await repo.GetPaginationListFromBodyAsync(request, ListSelector, criterias: criterias);
        }

        public async Task<DataTablePaginationResponseDto<VisitListDto>> GetMyCreatedVisitsAsync(DataTablePaginationRequestDto request)
        {
            var currentUserId = auditHelper.GetCurrentUserId();
            var repo = unitOfWork.GetRepository<Visit, int>();

            List<Expression<Func<Visit, bool>>> criterias = [x => x.CreatedBy == currentUserId];

            return await repo.GetPaginationListFromBodyAsync(request, ListSelector, criterias: criterias);
        }

        private static readonly Expression<Func<Visit, AdminVisitListDto>> AdminListSelector = x => new AdminVisitListDto
        {
            Id = x.Id,
            VisitDestinationName = x.VisitDestination.Name,
            DelegateName = x.Delegate.FullName,
            IsVisited = x.VisitedOn != null,
            VisitedOn = x.VisitedOn,
            CreatedById = x.CreatedBy,
            CreatedOn = x.CreatedOn
        };

        public async Task<DataTablePaginationResponseDto<AdminVisitListDto>> GetAllVisitsAdminAsync(DataTablePaginationRequestDto request)
        {
            var repo = unitOfWork.GetRepository<Visit, int>();
            var result = await repo.GetPaginationListFromBodyAsync(request, AdminListSelector);
            await AttachVisitCreatedByNamesAsync(result.Data);
            return result;
        }

        private async Task AttachVisitCreatedByNamesAsync(List<AdminVisitListDto> items)
        {
            var ids = items
                .Select(x => x.CreatedById)
                .Where(x => !string.IsNullOrWhiteSpace(x) && int.TryParse(x, out _))
                .Select(int.Parse)
                .Distinct()
                .ToList();

            if (ids.Count == 0) return;

            var names = await unitOfWork.GetRepository<ApplicationUser, int>().TableNoTrackingWithNoFilter
                .Where(x => ids.Contains(x.Id))
                .Select(x => new { x.Id, x.FullName })
                .ToDictionaryAsync(x => x.Id, x => x.FullName);

            foreach (var item in items)
            {
                if (int.TryParse(item.CreatedById, out var id) && names.TryGetValue(id, out var name))
                    item.CreatedByName = name;
            }
        }
    }
}
