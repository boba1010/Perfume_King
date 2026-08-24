namespace Perfume_King.Shared.Services
{
    public interface ITokenService
    {
        public Task<List<string?>> GetTokens();
        public Task<string?> GetToken();
        public Task<string?> GetRefreshToken();
        public Task SetTokens(string token, string refreshToken);
        public Task SetToken(string token);
        public Task SetRefreshToken(string token);
    }
}
