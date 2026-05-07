using Food_Market_BE.Modules.UserProfileModule.DTOs;

namespace Food_Market_BE.Modules.UserProfileModule.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileResponse> GetMeAsync(string userId);
        Task<UserProfileResponse> UpdateProfileAsync(string userId, UpdateUserProfileRequest request);
        Task ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task EnsureProfileExistsAsync(string userId, string username);
        //Task UpdateAvatarAsync(string userId, string avatarUrl);
    }
}
