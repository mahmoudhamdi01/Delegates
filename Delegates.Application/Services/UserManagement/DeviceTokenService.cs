using Delegates.Infrastructure.Entities.UserManagement;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.RegisterDevice;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Services.UserManagement
{
    public class DeviceTokenService(IUnitOfWork unitOfWork, IEntityAuditHelper auditHelper) : IDeviceTokenService
    {
        public async Task RegisterAsync(RegisterDeviceTokenDto dto)
        {
            var currentUserId = int.Parse(auditHelper.GetCurrentUserId()!);
            var repo = unitOfWork.GetRepository<DeviceToken, int>();

            var existing = await repo.TableNoTracking.FirstOrDefaultAsync(x => x.Token == dto.Token);
            if (existing != null)
            {
                if (existing.UserId == currentUserId) return;

                existing.UserId = currentUserId;
                existing.Platform = dto.Platform;
                auditHelper.SetUpdated(existing);
                repo.Update(existing);
            }
            else
            {
                var deviceToken = new DeviceToken { UserId = currentUserId, Token = dto.Token, Platform = dto.Platform };
                auditHelper.SetCreated(deviceToken);
                await repo.AddAsync(deviceToken);
            }

            await unitOfWork.SaveChangesAsync();
        }

        public async Task UnregisterAsync(string token)
        {
            var repo = unitOfWork.GetRepository<DeviceToken, int>();
            var existing = await repo.TableNoTracking.FirstOrDefaultAsync(x => x.Token == token);
            if (existing is null) return;

            auditHelper.SetSoftDeleted(existing);
            repo.Update(existing);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
