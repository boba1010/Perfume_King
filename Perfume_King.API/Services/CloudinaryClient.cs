using Microsoft.AspNetCore.DataProtection;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;

namespace Perfume_King.API.Services
{
    public class CloudinaryClient(HttpClient httpClient, string cloudName, string apiKey, string apiSecret)
    {
        public async Task<string?> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            // Base destination endpoint matching: https://cloudinary.com
            var url = $"https://api.cloudinary.com/v1_1/{cloudName}/image/upload";

            using var fileStream = file.OpenReadStream();
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);

            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            // Maps to the binary payload channel
            content.Add(streamContent, "file", file.FileName);

            // --- MANUALLY BUILDING HTTP BASIC AUTH ---
            // Converts "api_key:api_secret" into a secure base64 string header value
            var rawCredentials = $"{apiKey}:{apiSecret}";
            var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawCredentials));

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);
            request.Content = content;

            // Execute outbound request
            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<CloudinaryResponse>();
            return result?.SecureUrl;
        }

        private record CloudinaryResponse(
            [property: JsonPropertyName("secure_url")] string? SecureUrl
        );
    }
}
