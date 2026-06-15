using Food_Market_BE.Shared.Database;
using Food_Market_BE.Shared.Responses;
using Food_Market_BE.Shared.Seeder.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Food_Market_BE.Shared.Seeder.Controllers
{
    [ApiController]
    [Route("api/system")]
    public class SystemController : ControllerBase
    {
        private readonly IEnumerable<IDataSeeder> _seeders;
        private readonly MongoDbContext _dbContext;

        public SystemController(
            IEnumerable<IDataSeeder> seeders,
            MongoDbContext dbContext)
        {
            _seeders = seeders;
            _dbContext = dbContext;
        }

        [HttpPost("generate-fake-data")]
        public async Task<IActionResult> GenerateFakeData()
        {
            try
            {
                if (_seeders == null || !_seeders.Any())
                {
                    return BadRequest(
                        ApiResponse<string>.FailResponse("Không tìm thấy Seeder nào được đăng ký.")
                    );
                }

                var orderedSeeders = _seeders.OrderBy(x => x.Priority);

                foreach (var seeder in orderedSeeders)
                {
                    await seeder.SeedAsync();
                }

                return Ok(
                    ApiResponse<string>.SuccessResponse(null, "🎉 Tạo dữ liệu giả thành công!")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    ApiResponse<string>.FailResponse("Lỗi khi generate fake data", ex.Message)
                );
            }
        }

        [HttpDelete("reset-database")]
        public async Task<IActionResult> ResetDatabase()
        {
            try
            {
                // MongoDbContext không expose DB name nên cần lấy từ config nếu muốn drop DB
                // Cách đơn giản: bạn nên inject IConfiguration thay vì IMongoClient

                return Ok(
                    ApiResponse<string>.SuccessResponse(null,
                    "Bạn cần implement drop database trong MongoDbContext hoặc inject IConfiguration")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    ApiResponse<string>.FailResponse("Lỗi khi reset database", ex.Message)
                );
            }
        }
    }
}