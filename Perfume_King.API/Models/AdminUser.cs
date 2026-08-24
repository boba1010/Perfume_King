namespace Perfume_King.API.Models
{
    public class AdminUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public byte[] Salt { get; set; }
        public string Password { get; set; } = null!;
    }
}
