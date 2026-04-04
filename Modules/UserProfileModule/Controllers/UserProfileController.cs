using Food_Market_BE.Modules.UserProfileModule.DTOs;
using Food_Market_BE.Modules.UserProfileModule.Helpers;
using Food_Market_BE.Modules.UserProfileModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Food_Market_BE.Modules.UserProfileModule.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly IUserProfileService _service;
        private readonly FileUploadHelper _upload;

        public UserProfileController(IUserProfileService service, FileUploadHelper upload)
        {
            _service = service;
            _upload = upload;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = CurrentUserHelper.GetUserId(HttpContext);
            return Ok(await _service.GetMeAsync(userId));
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update(UpdateUserProfileRequest req)
        {
            var userId = CurrentUserHelper.GetUserId(HttpContext);
            return Ok(await _service.UpdateProfileAsync(userId, req));
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest req)
        {
            var userId = CurrentUserHelper.GetUserId(HttpContext);
            await _service.ChangePasswordAsync(userId, req);
            return Ok(new { message = "Password changed" });
        }

        [Authorize]
        [HttpPost("upload-avatar")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var url = await _upload.UploadAsync(file);
            return Ok(new UploadAvatarResponse { Url = url });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
