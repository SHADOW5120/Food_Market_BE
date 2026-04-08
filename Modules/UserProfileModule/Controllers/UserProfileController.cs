using Food_Market_BE.Modules.UserProfileModule.DTOs;
using Food_Market_BE.Modules.UserProfileModule.Helpers;
using Food_Market_BE.Modules.UserProfileModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Food_Market_BE.Modules.UserProfileModule.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserProfileController : ControllerBase
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
            var result = await _service.GetMeAsync(userId);
            return Ok(new { success = true, data = result });
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> Update(UpdateUserProfileRequest req)
        {
            var userId = CurrentUserHelper.GetUserId(HttpContext);
            var result = await _service.UpdateProfileAsync(userId, req);
            return Ok(new { success = true, data = result });
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest req)
        {
            var userId = CurrentUserHelper.GetUserId(HttpContext);
            await _service.ChangePasswordAsync(userId, req);
            return Ok(new { success = true, message = "Password changed successfully" });
        }

        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var url = await _upload.UploadAsync(file);
            return Ok(new { success = true, data = new { url = url } });
        }
    }
}