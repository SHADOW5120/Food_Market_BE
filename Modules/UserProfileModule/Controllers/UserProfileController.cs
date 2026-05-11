using Food_Market_BE.Modules.UserProfileModule.DTOs;
using Food_Market_BE.Modules.UserProfileModule.Helpers;
using Food_Market_BE.Modules.UserProfileModule.Services.Interfaces;
using Food_Market_BE.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Food_Market_BE.Modules.UserProfileModule.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _service;
        private readonly IFileUpDelService _upload;

        public UserProfileController(IUserProfileService service, IFileUpDelService upload)
        {
            _service = service;
            _upload = upload;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var userId = CurrentUserHelper.GetUserId(HttpContext);
                var result = await _service.GetMeAsync(userId);

                var response = ApiResponse<UserProfileResponse>.SuccessResponse(result);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> Update(UpdateUserProfileRequest req)
        {
            try
            {
                var userId = CurrentUserHelper.GetUserId(HttpContext);
                var result = await _service.UpdateProfileAsync(userId, req);

                var response = ApiResponse<UserProfileResponse>.SuccessResponse(result, "Cập nhật profile thành công");

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest req)
        {
            try
            {
                var userId = CurrentUserHelper.GetUserId(HttpContext);
                await _service.ChangePasswordAsync(userId, req);

                return Ok(ApiResponse<object>.SuccessResponse(null, "Password changed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResponse(ex.Message));
            }
        }

        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var url = await _upload.UploadAsync(file);

            return Ok(ApiResponse<object>.SuccessResponse(new { url }));
        }
    }
}