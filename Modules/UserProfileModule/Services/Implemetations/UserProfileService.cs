using Food_Market_BE.Modules.AuthModule.Helpers;
using Food_Market_BE.Modules.AuthModule.Repositories.Interfaces;
using Food_Market_BE.Modules.UserProfileModule.DTOs;
using Food_Market_BE.Modules.UserProfileModule.Models;
using Food_Market_BE.Modules.UserProfileModule.Repositories.Interfaces;
using Food_Market_BE.Modules.UserProfileModule.Services.Interfaces;

namespace Food_Market_BE.Modules.UserProfileModule.Services.Implemetations
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _profileRepository;
        private readonly IFileUpDelService _fileService;

        private readonly PasswordHasher _hasher;

        public UserProfileService(
            IUserRepository userRepository,
            IUserProfileRepository profileRepository,
            IFileUpDelService fileService,
            PasswordHasher hasher)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _fileService = fileService;
            _hasher = hasher;
        }

        public async Task EnsureProfileExistsAsync(string userId, string username)
        {
            var existing = await _profileRepository.GetByUserIdAsync(userId);
            if (existing != null) return;

            await _profileRepository.CreateAsync(new UserProfile
            {
                UserId = userId,
                Username = username ?? "",
                Phone = "",
                AvatarUrl = ""
            });
        }

        public async Task<UserProfileResponse> GetMeAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new Exception("User not found");

            var profile = await _profileRepository.GetByUserIdAsync(userId);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    Username = "",
                    Phone = "",
                    AvatarUrl = ""
                };

                await _profileRepository.CreateAsync(profile);
            }

            return new UserProfileResponse
            {
                UserId = user.Id,
                ProfileId = profile.Id,
                Email = user.Email,
                Username = profile.Username,
                Phone = profile.Phone,
                AvatarUrl = profile.AvatarUrl,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public async Task<UserProfileResponse> UpdateProfileAsync(string userId, UpdateUserProfileRequest req)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new Exception("User not found");

            var profile = await _profileRepository.GetByUserIdAsync(userId)
                ?? new UserProfile { UserId = userId };

            var oldAvatar = profile.AvatarUrl;

            profile.Username = req.Username ?? profile.Username;
            profile.Phone = req.Phone ?? profile.Phone;

            if (!string.IsNullOrEmpty(req.AvatarUrl))
            {
                profile.AvatarUrl = req.AvatarUrl;
            }

            if (profile.Id == null)
                await _profileRepository.CreateAsync(profile);
            else
                await _profileRepository.UpdateAsync(profile);

            if (!string.IsNullOrEmpty(req.AvatarUrl) && !string.IsNullOrEmpty(oldAvatar) && !string.Equals(oldAvatar, req.AvatarUrl, StringComparison.OrdinalIgnoreCase))
            {
                await _fileService.DeleteAsync(oldAvatar);
            }

            return new UserProfileResponse
            {
                UserId = user.Id,
                ProfileId = profile.Id,
                Email = user.Email,
                Username = profile.Username,
                Phone = profile.Phone,
                AvatarUrl = profile.AvatarUrl,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public async Task ChangePasswordAsync(string userId, ChangePasswordRequest req)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new Exception("User not found");

            if (!_hasher.Verify(req.CurrentPassword, user.PasswordHash))
                throw new Exception("Wrong password");

            if (req.NewPassword.Length < 6)
                throw new Exception("Password too short");

            if (req.NewPassword != req.ConfirmNewPassword)
                throw new Exception("Confirm mismatch");

            user.PasswordHash = _hasher.Hash(req.NewPassword);

            await _userRepository.UpdateAsync(user);
        }
    }
}
