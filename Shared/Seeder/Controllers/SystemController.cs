using Food_Market_BE.Shared.Responses;
using Food_Market_BE.Shared.Seeder.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Food_Market_BE.Shared.Seeder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemController : ControllerBase
    {
        private readonly IEnumerable<IDataSeeder> _seeders;

        private readonly IMongoClient _mongoClient;

        // Thêm IMongoClient vào Constructor
        public SystemController(IEnumerable<IDataSeeder> seeders, IMongoClient mongoClient)
        {
            _seeders = seeders;
            _mongoClient = mongoClient;
        }

        [HttpPost("generate-fake-data")]
        public async Task<IActionResult> GenerateFakeData()
        {
            try
            {
                if (!_seeders.Any())
                {
                    // Tận dụng ApiResponse<T> của bạn
                    return BadRequest(ApiResponse<string>.FailResponse("Không tìm thấy Seeder nào được đăng ký."));
                }

                // Đảm bảo chạy đúng thứ tự Priority
                var orderedSeeders = _seeders.OrderBy(s => s.Priority);

                foreach (var seeder in orderedSeeders)
                {
                    await seeder.SeedAsync();
                }

                return Ok(ApiResponse<string>.SuccessResponse(null, "🎉 Khởi tạo dữ liệu giả thành công!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Lỗi hệ thống khi sinh dữ liệu", ex.Message));
            }
        }

        [HttpDelete("reset-database")]
        public async Task<IActionResult> ResetDatabase()
        {
            try
            {
                // Thay "TenDatabaseCuaBan" bằng đúng tên DB bạn đã cấu hình ở Program.cs
                await _mongoClient.DropDatabaseAsync("TenDatabaseCuaBan");

                return Ok(ApiResponse<string>.SuccessResponse(null, "💣 Đã xóa trắng toàn bộ Database!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse("Lỗi khi xóa Database", ex.Message));
            }
        }
    }
}