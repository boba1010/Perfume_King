namespace Perfume_King.Shared.Contracts.Requests
{
    public class UserSignupRequest
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
