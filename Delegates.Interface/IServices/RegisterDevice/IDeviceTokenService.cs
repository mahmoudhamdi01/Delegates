using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.RegisterDevice
{
    public interface IDeviceTokenService
    {
        Task RegisterAsync(RegisterDeviceTokenDto dto);
        Task UnregisterAsync(string token);
    }
}
