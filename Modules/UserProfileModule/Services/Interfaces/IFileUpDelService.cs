namespace Food_Market_BE.Modules.UserProfileModule.Services.Interfaces
{
    public interface IFileUpDelService
    {
        Task<string> UploadAsync(IFormFile file);
        Task DeleteAsync(string fileUrl);
    }
}
