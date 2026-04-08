using Food_Market_BE.Modules.VoucherModule.DTOs;
using Food_Market_BE.Modules.VoucherModule.Helpers;
using Food_Market_BE.Modules.VoucherModule.Models;
using Food_Market_BE.Modules.VoucherModule.Repositories.Interfaces;
using Food_Market_BE.Modules.VoucherModule.Services.Interfaces;

namespace Food_Market_BE.Modules.VoucherModule.Services.Implementations
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherService(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<List<VoucherDto>> GetAvailableVouchersAsync()
        {
            var vouchers = await _voucherRepository.GetAllActiveVouchersAsync();

            return vouchers.Select(MapToDto).ToList();
        }

        public async Task<ApplyVoucherResponse> ApplyVoucherAsync(ApplyVoucherRequest request)
        {
            var voucher = await _voucherRepository.GetVoucherByCodeAsync(request.VoucherCode);

            if (voucher == null)
            {
                return new ApplyVoucherResponse
                {
                    IsValid = false,
                    Message = "Voucher không tồn tại",
                    CartTotal = request.CartTotal,
                    DiscountApplied = 0,
                    FinalTotal = request.CartTotal
                };
            }

            if (VoucherHelper.IsVoucherExpired(voucher))
            {
                return new ApplyVoucherResponse
                {
                    IsValid = false,
                    Message = "Voucher đã hết hạn",
                    VoucherCode = voucher.Code,
                    CartTotal = request.CartTotal,
                    DiscountApplied = 0,
                    FinalTotal = request.CartTotal
                };
            }

            if (VoucherHelper.IsUsageExceeded(voucher))
            {
                return new ApplyVoucherResponse
                {
                    IsValid = false,
                    Message = "Voucher đã hết lượt sử dụng",
                    VoucherCode = voucher.Code,
                    CartTotal = request.CartTotal,
                    DiscountApplied = 0,
                    FinalTotal = request.CartTotal
                };
            }

            if (!VoucherHelper.IsEligibleOrderAmount(voucher, request.CartTotal))
            {
                return new ApplyVoucherResponse
                {
                    IsValid = false,
                    Message = $"Đơn hàng tối thiểu phải từ {voucher.MinOrderAmount:N0}",
                    VoucherCode = voucher.Code,
                    CartTotal = request.CartTotal,
                    DiscountApplied = 0,
                    FinalTotal = request.CartTotal
                };
            }

            var discount = VoucherHelper.CalculateDiscount(voucher, request.CartTotal);
            var finalTotal = request.CartTotal - discount;

            return new ApplyVoucherResponse
            {
                IsValid = true,
                Message = "Áp dụng voucher thành công",
                VoucherCode = voucher.Code,
                CartTotal = request.CartTotal,
                DiscountApplied = discount,
                FinalTotal = finalTotal
            };
        }

        public async Task<VoucherDto> CreateVoucherAsync(CreateVoucherRequest request)
        {
            ValidateVoucherRequest(request);

            var existingVoucher = await _voucherRepository.GetVoucherByCodeAsync(request.Code);
            if (existingVoucher != null)
                throw new Exception("Voucher code đã tồn tại");

            var voucher = new Voucher
            {
                Code = request.Code.Trim().ToUpper(),
                Description = request.Description,
                DiscountAmount = request.DiscountAmount,
                DiscountPercent = request.DiscountPercent,
                ExpiryDate = request.ExpiryDate,
                MinOrderAmount = request.MinOrderAmount,
                MaxDiscountAmount = request.MaxDiscountAmount,
                MaxUsage = request.MaxUsage,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _voucherRepository.CreateVoucherAsync(voucher);

            return MapToDto(voucher);
        }

        public async Task<VoucherDto> UpdateVoucherAsync(string voucherId, CreateVoucherRequest request)
        {
            ValidateVoucherRequest(request);

            var voucher = await _voucherRepository.GetVoucherByIdAsync(voucherId);
            if (voucher == null)
                throw new Exception("Voucher không tồn tại");

            var existingVoucher = await _voucherRepository.GetVoucherByCodeAsync(request.Code);
            if (existingVoucher != null && existingVoucher.Id != voucherId)
                throw new Exception("Voucher code đã tồn tại");

            voucher.Code = request.Code.Trim().ToUpper();
            voucher.Description = request.Description;
            voucher.DiscountAmount = request.DiscountAmount;
            voucher.DiscountPercent = request.DiscountPercent;
            voucher.ExpiryDate = request.ExpiryDate;
            voucher.MinOrderAmount = request.MinOrderAmount;
            voucher.MaxDiscountAmount = request.MaxDiscountAmount;
            voucher.MaxUsage = request.MaxUsage;
            voucher.UpdatedAt = DateTime.UtcNow;

            await _voucherRepository.UpdateVoucherAsync(voucher);

            return MapToDto(voucher);
        }

        public async Task<bool> DeleteVoucherAsync(string voucherId)
        {
            var voucher = await _voucherRepository.GetVoucherByIdAsync(voucherId);
            if (voucher == null)
                return false;

            await _voucherRepository.DeleteVoucherAsync(voucherId);
            return true;
        }

        public async Task<VoucherDto?> GetVoucherByCodeAsync(string code)
        {
            var voucher = await _voucherRepository.GetVoucherByCodeAsync(code);
            if (voucher == null)
                return null;

            return MapToDto(voucher);
        }

        private static VoucherDto MapToDto(Voucher voucher)
        {
            return new VoucherDto
            {
                VoucherId = voucher.Id,
                Code = voucher.Code,
                Description = voucher.Description,
                DiscountAmount = voucher.DiscountAmount,
                DiscountPercent = voucher.DiscountPercent,
                ExpiryDate = voucher.ExpiryDate,
                MinOrderAmount = voucher.MinOrderAmount,
                MaxDiscountAmount = voucher.MaxDiscountAmount
            };
        }

        private static void ValidateVoucherRequest(CreateVoucherRequest request)
        {
            var hasAmount = request.DiscountAmount.HasValue && request.DiscountAmount.Value > 0;
            var hasPercent = request.DiscountPercent.HasValue && request.DiscountPercent.Value > 0;

            if (!hasAmount && !hasPercent)
                throw new Exception("Voucher phải có DiscountAmount hoặc DiscountPercent");

            if (hasAmount && hasPercent)
                throw new Exception("Voucher chỉ được có 1 loại giảm giá: số tiền hoặc phần trăm");

            if (request.ExpiryDate <= DateTime.UtcNow)
                throw new Exception("ExpiryDate phải lớn hơn thời gian hiện tại");
        }
    }
}
