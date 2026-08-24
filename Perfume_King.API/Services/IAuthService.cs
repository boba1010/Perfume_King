namespace Perfume_King.API.Services
{
    public interface IAuthService
    {
        public byte[] GenerateSalt();
        public string HashPassword(string password, byte[] salt);
        public bool VerifyPassword(string password, string hashed, byte[] salt);
        public string GenerateJwtToken(int userId, string email, string role);
        public string GenerateJwtToken(int userId, string role);
    }
}
