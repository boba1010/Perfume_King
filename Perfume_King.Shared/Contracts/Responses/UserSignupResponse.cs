namespace Perfume_King.Shared.Contracts.Responses
{
    public class UserSignupResponse
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
