using Food_Market_BE.Modules.VoucherModule.DTOs;

namespace Food_Market_BE.Modules.VoucherModule.Services.Interfaces
{
    public interface IVoucherService
    {
        Task<List<VoucherDto>> GetAvailableVouchersAsync();
        Task<VoucherDto?> GetVoucherByCodeAsync(string code);
        Task<ApplyVoucherResponse> ApplyVoucherAsync(ApplyVoucherRequest request);
        Task<VoucherDto> CreateVoucherAsync(CreateVoucherRequest request);
        Task<VoucherDto> UpdateVoucherAsync(string voucherId, CreateVoucherRequest request);
        Task<bool> DeleteVoucherAsync(string voucherId);
    }
}
