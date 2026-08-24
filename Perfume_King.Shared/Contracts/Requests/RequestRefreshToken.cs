namespace Perfume_King.Shared.Contracts.Requests
{
    public class RequestRefreshToken
    {
        public string Token { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
