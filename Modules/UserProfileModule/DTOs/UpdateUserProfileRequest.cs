namespace Food_Market_BE.Modules.UserProfileModule.DTOs
{
    public class UpdateUserProfileRequest
    {
        public string Username { get; set; }
        public string Phone { get; set; }
        public string AvatarUrl { get; set; }
    }
}
