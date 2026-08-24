using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.Contracts.Responses;
using System.Net;
using System.Net.Http.Json;

namespace Perfume_King.Shared.Services
{
    public class ApiClient(HttpClient httpClient, ITokenService tokenService)
    {
        private const string Uri = "https://localhost:7224/api/user/auth/refresh";

        public async Task<RefreshTokenResponse?> RefreshSessionAsync(RequestRefreshToken request)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync(Uri, request);

                if (!response.IsSuccessStatusCode)
                    return null;

                var result = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();
                return result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> SendAsync(Func<HttpClient, Task<HttpResponseMessage>> request, string role)
        {
            var token = await tokenService.GetToken();
            if (!string.IsNullOrEmpty(token))
                httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var response = await request(httpClient);
            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;

            var refreshToken = await tokenService.GetRefreshToken();

            var refreshed = await RefreshSessionAsync(new() { Token = refreshToken, Role = role });

            if (refreshed == null)
                return response;

            await tokenService.SetToken(refreshed.AccessToken);
            await tokenService.SetRefreshToken(refreshed.RefreshToken);

            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", refreshed.AccessToken);

            return await request(httpClient);
        }
    }
}
