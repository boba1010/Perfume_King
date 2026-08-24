using Perfume_King.Shared.Services;

namespace Perfume_King.Blazor.Services
{
    public class TokenService(ISecureStorage secureStorage) : ITokenService
    {
        private const string RefreshTokenKey = "RefreshToken";

        public async Task<string?> GetRefreshToken()
        {
            return await secureStorage.GetAsync(RefreshTokenKey);
        }

        public async Task<string?> GetToken()
        {
            return UserStateManager.AccessToken;
        }

        public async Task<List<string?>> GetTokens()
        {
            var refreshToken = await GetRefreshToken();
            var token = await GetToken();

            return [token, refreshToken];
        }

        public async Task SetRefreshToken(string token)
        {
            await secureStorage.SetAsync(RefreshTokenKey, token);
        }

        public async Task SetToken(string token)
        {
            UserStateManager.AccessToken = token;
        }

        public async Task SetTokens(string token, string refreshToken)
        {
            UserStateManager.AccessToken = token;
            await secureStorage.SetAsync(RefreshTokenKey, refreshToken);
        }
    }
}
