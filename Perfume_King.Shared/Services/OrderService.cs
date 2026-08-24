using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.DTOs;
using System.Net.Http.Json;

namespace Perfume_King.Shared.Services
{
    public class OrderService(ApiClient apiClient) : IOrderService
    {
        private const string Uri = "https://localhost:7224/api/orders";
        private const string AdminUri = "https://localhost:7224/api/admin/orders";

        private const string UserRole = "user";
        private const string AdminRole = "admin";

        public async Task<ServiceResult<OrderRequestDTO>> GetOrderAdminAsync(string orderNumber)
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.GetAsync($"{AdminUri}/{orderNumber}"), AdminRole);

                if (!response.IsSuccessStatusCode)
                    return ServiceResult<OrderRequestDTO>.Failure("Error: Something wrong has happened while receiving the data");

                var result = await response.Content.ReadFromJsonAsync<OrderRequestDTO>();
                return ServiceResult<OrderRequestDTO>.Success(result);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<OrderRequestDTO>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult<OrderRequestDTO>.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult<OrderRequestDTO>.Failure($"Error: An unexpected system error occurred.");
            }
        }

        public async Task<ServiceResult<OrderRequestDTO>> GetOrderAsync(string orderNum)
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.GetAsync($"{Uri}/{orderNum}"), UserRole);

                if (!response.IsSuccessStatusCode)
                    return ServiceResult<OrderRequestDTO>.Failure("Error: Something wrong has happened while receiving the data");

                var result = await response.Content.ReadFromJsonAsync<OrderRequestDTO>();
                return ServiceResult<OrderRequestDTO>.Success(result);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<OrderRequestDTO>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult<OrderRequestDTO>.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult<OrderRequestDTO>.Failure($"Error: An unexpected system error occurred.");
            }
        }

        public async Task<ServiceResult<List<OrderRequestDTO>>> GetOrdersAdminAsync()
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.GetAsync(AdminUri), AdminRole);

                if (!response.IsSuccessStatusCode)
                    return ServiceResult<List<OrderRequestDTO>>.Failure("Error: Something wrong has happened while receiving the data");

                var result = await response.Content.ReadFromJsonAsync<List<OrderRequestDTO>>();
                return ServiceResult<List<OrderRequestDTO>>.Success(result);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<List<OrderRequestDTO>>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult<List<OrderRequestDTO>>.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult<List<OrderRequestDTO>>.Failure($"Error: An unexpected system error occurred.");
            }
        }

        public async Task<ServiceResult<List<OrderRequestDTO>>> GetOrdersAsync()
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.GetAsync(Uri), UserRole);

                if (!response.IsSuccessStatusCode)
                    return ServiceResult<List<OrderRequestDTO>>.Failure("Error: Something wrong has happened while receiving the data");

                var result = await response.Content.ReadFromJsonAsync<List<OrderRequestDTO>>();
                return ServiceResult<List<OrderRequestDTO>>.Success(result);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<List<OrderRequestDTO>>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult<List<OrderRequestDTO>>.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult<List<OrderRequestDTO>>.Failure($"Error: An unexpected system error occurred.");
            }
        }

        public async Task<ServiceResult> RequestOrderAsync(PaymentMethod paymentMethod)
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.PostAsJsonAsync($"{Uri}/request-order", paymentMethod), UserRole);

                if (!response.IsSuccessStatusCode)
                    return ServiceResult.Failure("Error: Something wrong has happened while receiving the data");

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

        public async Task<ServiceResult> UpdateOrderAsync(RequestOrderStateUpdate request)
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.PostAsJsonAsync($"{AdminUri}/update/state", request), AdminRole);

                if (!response.IsSuccessStatusCode)
                    return ServiceResult.Failure("Error: Something wrong has happened while receiving the data");

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
