using System.Security.Cryptography;

namespace Food_Market_BE.Modules.AuthModule.Helpers
{
    public class TokenGenerator
    {
        public string Generate()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
