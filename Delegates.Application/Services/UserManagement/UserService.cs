using Delegates.Infrastructure.Entities.UserManagement;
using Delegates.Infrastructure.Enums.UserManagement;
using Delegates.Infrastructure.Exceptions;
using Delegates.Infrastructure.Shared;
using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.UserManagement.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Services.UserManagement
{
    public class UserService(
        IUnitOfWork unitOfWork,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IEntityAuditHelper auditHelper,
        ICurrentUserContext currentUserContext) : IUserService
    {
        public async Task<ProfileDto> GetProfileAsync()
        {
            var user = await GetCurrentUserAsync();

            return new ProfileDto
            {
                FullName = user.FullName,
                Code = user.Code,
                PhoneNumber = user.PhoneNumber,
                SecondaryPhoneNumber = user.SecondaryPhoneNumber,
                Address = user.Address,
                UserType = user.UserType
            };
        }

        public async Task ChangePasswordAsync(ChangePasswordRequestDto request)
        {
            var repo = unitOfWork.GetRepository<ApplicationUser, int>();
            var user = await GetCurrentUserAsync();

            var verify = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
            if (verify == PasswordVerificationResult.Failed)
                throw new BadRequestException(["كلمة المرور الحالية غير صحيحة"]);

            user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
            auditHelper.SetUpdated(user);
            repo.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateSecondaryPhoneAsync(UpdateSecondaryPhoneRequestDto request)
        {
            var repo = unitOfWork.GetRepository<ApplicationUser, int>();
            var user = await GetCurrentUserAsync();

            user.SecondaryPhoneNumber = request.SecondaryPhoneNumber;
            auditHelper.SetUpdated(user);
            repo.Update(user);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<DataTablePaginationResponseDto<UserListItemDto>> GetUsersAsync(DataTablePaginationRequestDto request)
        {
            var repo = unitOfWork.GetRepository<ApplicationUser, int>();

            var result = await repo.GetPaginationListFromBodyAsync(
                request,
                selector: x => new UserListItemDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    PhoneNumber = x.PhoneNumber,
                    UserType = x.UserType,
                    CreatedOn = x.CreatedOn,
                    CreatedById = x.CreatedBy
                });

            await AttachCreatedByNamesAsync(result.Data);
            return result;
        }

        public async Task<UserDetailsDto> GetUserDetailsAsync(int id)
        {
            var repo = unitOfWork.GetRepository<ApplicationUser, int>();
            var user = await repo.GetByIdAsync(id) ?? throw new UserNotFoundByIdException(id);

            var dto = new UserDetailsDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Code = user.Code,
                PhoneNumber = user.PhoneNumber,
                SecondaryPhoneNumber = user.SecondaryPhoneNumber,
                Address = user.Address,
                UserType = user.UserType,
                CreatedOn = user.CreatedOn,
                CreatedById = user.CreatedBy
            };

            await AttachCreatedByNamesAsync([dto]);
            return dto;
        }

        public async Task<UserDetailsDto> CreateUserAsync(CreateUserRequestDto request)
        {
            var usersRepo = unitOfWork.GetRepository<ApplicationUser, int>();

            var phoneExists = await usersRepo.TableNoTracking.AnyAsync(x => x.PhoneNumber == request.PhoneNumber);
            if (phoneExists)
                throw new BadRequestException(["رقم الهاتف مستخدم بالفعل"]);

            var isSuperAdmin = currentUserContext.IsSuperAdmin;
            int? targetAccountId;

            if (isSuperAdmin && request.UserType == UserType.Admin)
            {
                if (string.IsNullOrWhiteSpace(request.AccountName))
                    throw new BadRequestException(["اسم الحساب/الشركة مطلوب عند إنشاء أدمن جديد"]);

                var accountsRepo = unitOfWork.GetRepository<Account, int>();
                var account = new Account { CompanyName = request.AccountName };
                auditHelper.SetCreated(account); // AccountId هيفضل null هنا، وده صح لأنه هو نفسه الـ Tenant
                await accountsRepo.AddAsync(account);
                await unitOfWork.SaveChangesAsync(); // نحتاج نحفظ عشان ناخد الـ Id بتاعه

                targetAccountId = account.Id;
            }
            else if (!isSuperAdmin)
            {
                if (request.UserType is UserType.SuperAdmin or UserType.Admin)
                    throw new BadRequestException(["غير مسموح لك بإنشاء هذا النوع من المستخدمين"]);

                targetAccountId = currentUserContext.AccountId;
            }
            else
            {
                throw new BadRequestException(["نوع مستخدم غير صالح"]);
            }

            var newUser = new ApplicationUser
            {
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                SecondaryPhoneNumber = request.SecondaryPhoneNumber,
                Address = request.Address,
                UserType = request.UserType,
                AccountId = targetAccountId,
                Code = await GenerateUserCodeAsync()
            };
            newUser.PasswordHash = passwordHasher.HashPassword(newUser, request.Password);
            auditHelper.SetCreated(newUser);

            await usersRepo.AddAsync(newUser);
            await unitOfWork.SaveChangesAsync();

            return await GetUserDetailsAsync(newUser.Id);
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var userId = int.Parse(auditHelper.GetCurrentUserId()!);
            var repo = unitOfWork.GetRepository<ApplicationUser, int>();
            return await repo.GetByIdAsync(userId) ?? throw new UserNotFoundByIdException(userId);
        }

        private async Task<string> GenerateUserCodeAsync()
        {
            var repo = unitOfWork.GetRepository<ApplicationUser, int>();
            var count = await repo.TableNoTrackingWithNoFilter.CountAsync();
            return $"U-{count + 1:D5}";
        }

        private async Task AttachCreatedByNamesAsync(IEnumerable<dynamic> items)
        {
            var ids = ((IEnumerable<object>)items)
                .Select(x => (string?)((dynamic)x).CreatedById)
                .Where(x => !string.IsNullOrWhiteSpace(x) && int.TryParse(x, out _))
                .Select(int.Parse)
                .Distinct()
                .ToList();

            if (ids.Count == 0) return;

            var repo = unitOfWork.GetRepository<ApplicationUser, int>();
            var names = await repo.TableNoTrackingWithNoFilter
                .Where(x => ids.Contains(x.Id))
                .Select(x => new { x.Id, x.FullName })
                .ToDictionaryAsync(x => x.Id, x => x.FullName);

            foreach (dynamic item in items)
            {
                if (int.TryParse((string?)item.CreatedById, out var id) && names.TryGetValue(id, out var name))
                    item.CreatedByName = name;
            }
        }
    }
}
