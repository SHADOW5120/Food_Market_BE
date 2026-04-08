using Food_Market_BE.Modules.VoucherModule.DTOs;
using Food_Market_BE.Modules.VoucherModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Food_Market_BE.Modules.VoucherModule.Controllers
{
    [ApiController]
    [Route("api/vouchers")]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableVouchers()
        {
            var vouchers = await _voucherService.GetAvailableVouchersAsync();
            return Ok(new { success = true, data = vouchers });
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetVoucherByCode(string code)
        {
            var voucher = await _voucherService.GetVoucherByCodeAsync(code);
            if (voucher == null)
                return Ok(new { success = false, error = "Voucher not found" });

            return Ok(new { success = true, data = voucher });
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyVoucher([FromBody] ApplyVoucherRequest request)
        {
            var result = await _voucherService.ApplyVoucherAsync(request);
            return Ok(new { success = result.IsValid, data = result, message = result.Message });
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveVoucher()
        {
            return Ok(new { success = true, message = "Voucher removed successfully" });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherRequest request)
        {
            var result = await _voucherService.CreateVoucherAsync(request);
            return Ok(new { success = true, data = result, message = "Voucher created successfully" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVoucher(string id, [FromBody] CreateVoucherRequest request)
        {
            var result = await _voucherService.UpdateVoucherAsync(id, request);
            return Ok(new { success = true, data = result, message = "Voucher updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVoucher(string id)
        {
            var deleted = await _voucherService.DeleteVoucherAsync(id);
            if (!deleted) return NotFound(new { success = false, message = "Voucher không tồn tại" });
            return Ok(new { success = true, message = "Xóa voucher thành công" });
        }
    }
}