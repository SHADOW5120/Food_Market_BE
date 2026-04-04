namespace Food_Market_BE.Modules.AuthModule.DTOs
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }

    }
}
