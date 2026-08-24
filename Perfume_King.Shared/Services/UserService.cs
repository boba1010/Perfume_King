using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.Contracts.Responses;
using Perfume_King.Shared.DTOs;
using System.Net.Http.Json;

namespace Perfume_King.Shared.Services
{
    public class UserService(ApiClient apiClient) : IUserService
    {
        private const string Uri = "https://localhost:7224/api/user";
        private const string AdminUri = "https://localhost:7224/api/admin/users";

        private const string UserRole = "user";
        private const string AdminRole = "admin";

        public async Task<ServiceResult<UserProfileDTO>> GetUserProfileAsync()
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.GetAsync($"{Uri}/profile"), UserRole);

                if (!response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    return ServiceResult<UserProfileDTO>.Failure(msg);
                }

                var result = await response.Content.ReadFromJsonAsync<UserProfileDTO>();
                return ServiceResult<UserProfileDTO>.Success(result);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<UserProfileDTO>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult<UserProfileDTO>.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult<UserProfileDTO>.Failure($"Error: An unexpected system error occurred.");
            }
        }

        public async Task<ServiceResult> UpdateUserProfileAsync(RequestUserProfileUpdate request)
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.PostAsJsonAsync($"{Uri}/profile/update", request), UserRole);

                if (!response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    return ServiceResult.Failure(msg);
                }

                return ServiceResult.Success();
            }
            catch (TaskCanceledException)
            {
                return ServiceResult.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult.Failure($"Error: An unexpected system error occurred.");
            }
        }

        public async Task<ServiceResult<List<UserDTO>>> GetUsersAsync()
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.GetAsync(AdminUri), AdminRole);

                if (!response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    return ServiceResult<List<UserDTO>>.Failure(msg);
                }

                var result = await response.Content.ReadFromJsonAsync<List<UserDTO>>();
                return ServiceResult<List<UserDTO>>.Success(result);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<List<UserDTO>>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult<List<UserDTO>>.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult<List<UserDTO>>.Failure($"Error: An unexpected system error occurred.");
            }
        }
    }
}
