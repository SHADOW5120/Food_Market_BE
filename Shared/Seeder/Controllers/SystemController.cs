using Food_Market_BE.Shared.Seeder.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Food_Market_BE.Shared.Seeder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemController
    {
        private readonly IEnumerable<IDataSeeder> _seeders;

        public SystemController(IEnumerable<IDataSeeder> seeders)
        {
            _seeders = seeders;
        }

        //[HttpPost("generate-fake-data")]
        //public async Task<IActionResult> GenerateFakeData()
        //{
        //    try
        //    {
        //        if (!_seeders.Any())
        //            return BadRequest("Không tìm thấy Seeder nào được đăng ký trong hệ thống.");

        //        // Sắp xếp các Seeder theo Priority từ nhỏ đến lớn (1 -> 6)
        //        var orderedSeeders = _seeders.OrderBy(s => s.Priority);

        //        foreach (var seeder in orderedSeeders)
        //        {
        //            await seeder.SeedAsync();
        //        }

        //        return Ok(new { Message = "🎉 Khởi tạo dữ liệu giả thành công!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Error = "Lỗi hệ thống khi sinh dữ liệu: " + ex.Message });
        //    }
        //}
    }
}
