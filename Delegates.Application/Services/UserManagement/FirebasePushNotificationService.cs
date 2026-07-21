using Delegates.Infrastructure.Entities.UserManagement;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.RegisterDevice;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Services.UserManagement
{
    public class FirebasePushNotificationService(IUnitOfWork unitOfWork, IEntityAuditHelper auditHelper) : IPushNotificationService
    {
        public async Task NotifyUserAsync(int userId, string title, string body)
        {
            var tokensRepo = unitOfWork.GetRepository<DeviceToken, int>();
            var tokens = await tokensRepo.TableNoTracking
                .Where(x => x.UserId == userId)
                .Select(x => x.Token)
                .ToListAsync();

            if (tokens.Count == 0) return;

            var invalidTokens = new List<string>();

            foreach (var token in tokens)
            {
                try
                {
                    var message = new Message
                    {
                        Token = token,
                        Notification = new Notification { Title = title, Body = body }
                    };

                    await FirebaseMessaging.DefaultInstance.SendAsync(message);
                }
                catch (FirebaseMessagingException ex) when (
                    ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                    ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
                {
                    invalidTokens.Add(token);
                }
                catch (Exception) { }
            }

            if (invalidTokens.Count > 0)
            {
                var invalidEntities = await tokensRepo.TableNoTracking
                    .Where(x => invalidTokens.Contains(x.Token))
                    .ToListAsync();

                foreach (var entity in invalidEntities)
                {
                    auditHelper.SetSoftDeleted(entity);
                    tokensRepo.Update(entity);
                }

                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}
