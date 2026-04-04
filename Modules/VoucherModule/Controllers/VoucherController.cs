using Food_Market_BE.Modules.VoucherModule.DTOs;
using Food_Market_BE.Modules.VoucherModule.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Food_Market_BE.Modules.VoucherModule.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var result = await _voucherService.GetAvailableVouchersAsync();
            return Ok(result);
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyVoucher([FromBody] ApplyVoucherRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _voucherService.ApplyVoucherAsync(request);
            return Ok(result);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _voucherService.CreateVoucherAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVoucher(string id, [FromBody] CreateVoucherRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _voucherService.UpdateVoucherAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVoucher(string id)
        {
            var deleted = await _voucherService.DeleteVoucherAsync(id);

            if (!deleted)
                return NotFound(new { message = "Voucher không tồn tại" });

            return Ok(new { message = "Xóa voucher thành công" });
        }
    }
}
