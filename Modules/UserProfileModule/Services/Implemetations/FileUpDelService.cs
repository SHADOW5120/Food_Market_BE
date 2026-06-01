using Food_Market_BE.Modules.UserProfileModule.Services.Interfaces;

namespace Food_Market_BE.Modules.UserProfileModule.Services.Implementations
{
    public class FileUpDelService : IFileUpDelService
    {
        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty", nameof(file));

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowed.Contains(extension))
                throw new ArgumentException("Only image files are allowed", nameof(file));

            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var uploadPath = Path.Combine(rootPath, "uploads");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return a relative path so the caller can compose the full URL (avoids hardcoded host).
            return $"/uploads/{fileName}";
        }

        public async Task DeleteAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return;

            try
            {
                var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);

                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var filePath = Path.Combine(rootPath, "uploads", fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete file error: {ex.Message}");
            }

            await Task.CompletedTask;
        }
    }
}