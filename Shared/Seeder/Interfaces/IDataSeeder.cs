namespace Food_Market_BE.Shared.Seeder.Interfaces
{
    public interface IDataSeeder
    {
        // Độ ưu tiên: Số càng nhỏ chạy càng sớm. Nhóm 1 sẽ có Priority = 1.
        int Priority { get; }

        // Hàm chứa logic sinh dữ liệu
        Task SeedAsync();
    }
}
