using Food_Market_BE.Modules.VoucherModule.Models;

namespace Food_Market_BE.Modules.VoucherModule.Repositories.Interfaces
{
    public interface IVoucherRepository
    {
        Task<List<Voucher>> GetAllActiveVouchersAsync();
        Task<Voucher?> GetVoucherByIdAsync(string voucherId);
        Task<Voucher?> GetVoucherByCodeAsync(string code);
        Task CreateVoucherAsync(Voucher voucher);
        Task UpdateVoucherAsync(Voucher voucher);
        Task DeleteVoucherAsync(string voucherId);
    }
}
