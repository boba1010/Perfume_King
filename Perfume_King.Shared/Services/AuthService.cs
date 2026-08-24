using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.Contracts.Responses;
using Perfume_King.Shared.DTOs;
using System.Net.Http.Json;

namespace Perfume_King.Shared.Services
{
    public class AuthService(ApiClient apiClient, ITokenService tokenService) : IAuthService
    {
        private const string Uri = "https://localhost:7224/api/user";
        private const string AdminUri = "https://localhost:7224/api/admin";

        private const string UserRole = "user";
        private const string AdminRole = "admin";

        public async Task<ServiceResult<UserLoginResponse>> LoginAsync(UserLoginRequest request)
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.PostAsJsonAsync($"{Uri}/auth/login", request), UserRole);

                if (!response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    return ServiceResult<UserLoginResponse>.Failure(msg);
                }

                var result = await response.Content.ReadFromJsonAsync<UserLoginResponse>();
                await tokenService.SetToken(result.Token);
                await tokenService.SetRefreshToken(result.RefreshToken);
                return ServiceResult<UserLoginResponse>.Success(result);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<UserLoginResponse>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult<UserLoginResponse>.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult<UserLoginResponse>.Failure($"Error: An unexpected system error occurred.");
            }
        }

        public async Task<ServiceResult<UserSignupResponse>> SignupAsync(UserSignupRequest request)
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.PostAsJsonAsync($"{Uri}/auth/signup", request), UserRole);

                if (!response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    return ServiceResult<UserSignupResponse>.Failure(msg);
                }

                var result = await response.Content.ReadFromJsonAsync<UserSignupResponse>();
                await tokenService.SetToken(result.Token);
                await tokenService.SetRefreshToken(result.RefreshToken);
                return ServiceResult<UserSignupResponse>.Success(result);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<UserSignupResponse>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return ServiceResult<UserSignupResponse>.Failure("Error: Network connection failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ServiceResult<UserSignupResponse>.Failure($"Error: An unexpected system error occurred.");
            }
        }

        public async Task<ServiceResult<bool>> VerifyUserAsync()
        {
            try
            {
                var result = await apiClient.SendAsync(client => client.GetAsync($"{Uri}/auth/verify"), UserRole);

                return ServiceResult<bool>.Success(result.IsSuccessStatusCode);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<bool>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult<bool>.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult<bool>.Failure($"Error: An unexpected system error occurred.");
            }
        }

        public async Task<ServiceResult<AdminLoginResponse>> AdminLoginAsync(RequestAdminLogin request)
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.PostAsJsonAsync($"{AdminUri}/auth/login", request), null);

                if (!response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    return ServiceResult<AdminLoginResponse>.Failure(msg);
                }

                var result = await response.Content.ReadFromJsonAsync<AdminLoginResponse>();
                await tokenService.SetToken(result.Token);
                await tokenService.SetRefreshToken(result.RefreshToken);
                return ServiceResult<AdminLoginResponse>.Success(result);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<AdminLoginResponse>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return ServiceResult<AdminLoginResponse>.Failure("Error: Network connection failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ServiceResult<AdminLoginResponse>.Failure($"Error: An unexpected system error occurred.");
            }
        }

        public async Task<ServiceResult<bool>> VerifyAdminAsync()
        {
            try
            {
                var result = await apiClient.SendAsync(client => client.GetAsync($"{AdminUri}/auth/verify"), AdminRole);

                return ServiceResult<bool>.Success(result.IsSuccessStatusCode);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<bool>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult<bool>.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult<bool>.Failure($"Error: An unexpected system error occurred.");
            }
        }
    }
}
