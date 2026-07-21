using Delegates.Infrastructure.Entities.UserManagement;
using Delegates.Infrastructure.Exceptions;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.UserManagement.Login;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Services.UserManagement
{
    public class AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IJwtTokenService jwtTokenService) : IAuthService
    {
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var usersRepo = unitOfWork.GetRepository<ApplicationUser, int>();
            var user = await usersRepo.TableNoTrackingWithNoFilter
                .FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber && !x.IsDeleted);

            if (user is null)
                throw new UnAuthorizedException();

            var verifyResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verifyResult == PasswordVerificationResult.Failed)
                throw new UnAuthorizedException();

            return new LoginResponseDto
            {
                Token = jwtTokenService.GenerateToken(user),
                FullName = user.FullName,
                Code = user.Code,
                UserType = user.UserType
            };
        }
    }
}
