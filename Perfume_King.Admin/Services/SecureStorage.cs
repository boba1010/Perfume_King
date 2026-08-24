using Perfume_King.Shared.Services;

namespace Perfume_King.Admin.Services
{
    public class SecureStorage(ILocalStorage localStorage) : ISecureStorage
    {
        public async Task Clear()
        {
        }

        public async Task<string?> GetAsync(string key)
        {
            return await localStorage.GetAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await localStorage.RemoveAsync(key);
        }

        public async Task SetAsync(string key, string value)
        {
            await localStorage.SetAsync(key, value);
        }
    }
}
