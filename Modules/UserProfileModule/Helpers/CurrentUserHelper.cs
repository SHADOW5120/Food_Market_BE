using System.Security.Claims;

namespace Food_Market_BE.Modules.UserProfileModule.Helpers
{
    public static class CurrentUserHelper
    {
        public static string GetUserId(HttpContext context)
        {
            return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetEmail(HttpContext context)
        {
            return context.User.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string GetRole(HttpContext context)
        {
            return context.User.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
