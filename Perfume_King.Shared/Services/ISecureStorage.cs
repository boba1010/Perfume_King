namespace Perfume_King.Shared.Services
{
    public interface ISecureStorage
    {
        public Task<string?> GetAsync(string key);
        public Task SetAsync(string key, string value);
        public Task RemoveAsync(string key);
        public Task Clear();
    }
}
