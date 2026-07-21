using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.RegisterDevice
{
    public interface IPushNotificationService
    {
        Task NotifyUserAsync(int userId, string title, string body);
    }
}
