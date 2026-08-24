namespace Perfume_King.Shared.Contracts.Requests
{
    public class AdminSignupRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string SignupToken { get; set; } = null!;
    }
}
