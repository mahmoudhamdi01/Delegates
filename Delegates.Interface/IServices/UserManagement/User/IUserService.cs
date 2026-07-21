using Delegates.Infrastructure.Shared.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.UserManagement.User
{
    public interface IUserService
    {
        Task<ProfileDto> GetProfileAsync();
        Task ChangePasswordAsync(ChangePasswordRequestDto request);
        Task UpdateSecondaryPhoneAsync(UpdateSecondaryPhoneRequestDto request);
        Task<DataTablePaginationResponseDto<UserListItemDto>> GetUsersAsync(DataTablePaginationRequestDto request);
        Task<UserDetailsDto> GetUserDetailsAsync(int id);
        Task<UserDetailsDto> CreateUserAsync(CreateUserRequestDto request);
    }
}
