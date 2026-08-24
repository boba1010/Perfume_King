using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.Contracts.Responses;
using Perfume_King.Shared.DTOs;

namespace Perfume_King.Shared.Services
{
    public interface IAuthService
    {
        public Task<ServiceResult<UserLoginResponse>> LoginAsync(UserLoginRequest request);
        public Task<ServiceResult<UserSignupResponse>> SignupAsync(UserSignupRequest request);
        public Task<ServiceResult<bool>> VerifyUserAsync();

        public Task<ServiceResult<AdminLoginResponse>> AdminLoginAsync(RequestAdminLogin request);
        public Task<ServiceResult<bool>> VerifyAdminAsync();
    }
}
