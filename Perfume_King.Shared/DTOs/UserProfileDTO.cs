namespace Perfume_King.Shared.DTOs
{
    public class UserProfileDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string City { get; set; } = "Cairo";
        public string Address { get; set; } = string.Empty;

        public List<ProfileOrderSummaryDTO> Orders { get; set; } = [];
    }
}
