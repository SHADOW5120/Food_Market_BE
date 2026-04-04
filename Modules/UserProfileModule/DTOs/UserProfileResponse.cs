namespace Food_Market_BE.Modules.UserProfileModule.DTOs
{
    public class UserProfileResponse
    {

        public string UserId { get; set; }
        public string ProfileId { get; set; }

        public string Email { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string AvatarUrl { get; set; }

        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}
