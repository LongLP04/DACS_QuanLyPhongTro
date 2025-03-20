using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    public class PhuongThucThanhToan
    {
        [Key]
        public int MaPhuongThuc { get; set; }
        public string TenPhuongThuc { get; set; } = string.Empty;
        public string? MoTa { get; set; }

        // Danh sách các phiếu thanh toán sử dụng phương thức này
        public ICollection<PhieuThanhToan> PhieuThanhToans { get; set; } = new List<PhieuThanhToan>();
    }

}
