namespace Food_Market_BE.Modules.AuthModule.DTOs
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public object User { get; set; }

    }
}
