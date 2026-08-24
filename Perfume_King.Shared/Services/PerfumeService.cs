using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.DTOs;
using System.Net.Http.Json;

namespace Perfume_King.Shared.Services
{
    public class PerfumeService(ApiClient apiClient) : IPerfumeService
    {
        private const string Uri = "https://localhost:7224/api/perfumes";
        private const string AdminUri = "https://localhost:7224/api/admin/perfumes";

        private const string AdminRole = "admin";

        public async Task<ServiceResult> DeleteAllPerfumesAsync()
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.PostAsync($"{AdminUri}/delete-all", null), AdminRole);

                if (!response.IsSuccessStatusCode)
                    return ServiceResult.Failure("Error: Something wrong has happened while deleting all perfumes.");

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

        public async Task<ServiceResult> DeletePerfumeAsync(int perfumeId)
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.PostAsJsonAsync($"{AdminUri}/delete", perfumeId), AdminRole);

                if (!response.IsSuccessStatusCode)
                    return ServiceResult.Failure("Error: Something wrong has happened while deleting the perfume.");

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

        public async Task<ServiceResult<List<PerfumeDTO>>> GetFeaturedPerfumesAsync()
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.GetAsync($"{Uri}/featured-perfumes"), null);

                if (!response.IsSuccessStatusCode)
                    return ServiceResult<List<PerfumeDTO>>.Failure("Error: Something wrong has happened while receiving the data");

                var result = await response.Content.ReadFromJsonAsync<List<PerfumeDTO>>();
                return ServiceResult<List<PerfumeDTO>>.Success(result);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<List<PerfumeDTO>>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult<List<PerfumeDTO>>.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult<List<PerfumeDTO>>.Failure($"Error: An unexpected system error occurred.");
            }
        }

        public async Task<ServiceResult<PerfumeDTO>> GetPerfumeAsync(int id)
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.GetAsync($"{Uri}/{id}"), null);

                if (!response.IsSuccessStatusCode)
                    return ServiceResult<PerfumeDTO>.Failure("Error: Something wrong has happened while deleting the perfume.");

                var result = await response.Content.ReadFromJsonAsync<PerfumeDTO>();
                return ServiceResult<PerfumeDTO>.Success(result);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<PerfumeDTO>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult<PerfumeDTO>.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult<PerfumeDTO>.Failure($"Error: An unexpected system error occurred.");
            }
        }

        public async Task<ServiceResult<List<PerfumeDTO>>> GetPerfumesAsync()
        {
            try
            {
                var response = await apiClient.SendAsync(client => client.GetAsync(Uri), null);

                if (!response.IsSuccessStatusCode)
                    return ServiceResult<List<PerfumeDTO>>.Failure("Error: Something wrong has happened while receiving the data");

                var result = await response.Content.ReadFromJsonAsync<List<PerfumeDTO>>();
                return ServiceResult<List<PerfumeDTO>>.Success(result);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<List<PerfumeDTO>>.Failure("Error: Connection Timeout.");
            }
            catch (HttpRequestException)
            {
                return ServiceResult<List<PerfumeDTO>>.Failure("Error: Network connection failed.");
            }
            catch (Exception)
            {
                return ServiceResult<List<PerfumeDTO>>.Failure($"Error: An unexpected system error occurred.");
            }
        }
    }
}
