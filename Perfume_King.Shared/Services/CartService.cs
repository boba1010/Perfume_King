using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.DTOs;
using System.Net.Http.Json;

namespace Perfume_King.Shared.Services
{
    public class CartService(ApiClient apiClient) : ICartService
    {
        private const string Uri = "https://localhost:7224/api/cart";

        public async Task<ServiceResult> AddPerfumeToCartAsync(int id)
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.PostAsJsonAsync($"{Uri}/add", id), "user");

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

        public async Task<ServiceResult> ChangeCartItemQtyAsync(RequestCartItemQtyChange request)
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.PostAsJsonAsync($"{Uri}/change-item-qty", request), "user");

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

        public async Task<ServiceResult<CartDTO>> GetCartAsync()
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.GetAsync(Uri), "user");

                if (!response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    return ServiceResult<CartDTO>.Failure(msg);
                }

                var result = await response.Content.ReadFromJsonAsync<CartDTO>();
                return ServiceResult<CartDTO>.Success(result);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<CartDTO>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult<CartDTO>.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult<CartDTO>.Failure($"Error: An unexpected system error occurred.");
            }
        }

        public async Task<ServiceResult> RemoveCartItemAsync(int id)
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.PostAsJsonAsync($"{Uri}/remove", id), "user");

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
    }
}
