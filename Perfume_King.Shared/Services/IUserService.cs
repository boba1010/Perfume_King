using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.DTOs;

namespace Perfume_King.Shared.Services
{
    public interface IUserService
    {
        public Task<ServiceResult<UserProfileDTO>> GetUserProfileAsync();
        public Task<ServiceResult> UpdateUserProfileAsync(RequestUserProfileUpdate request);
        public Task<ServiceResult<List<UserDTO>>> GetUsersAsync();
    }
}
