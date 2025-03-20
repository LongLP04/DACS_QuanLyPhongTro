using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    public class PhieuThanhToan
    {

        [Key]   
        public int MaPhieuThanhToan { get; set; }
        public decimal SoTienThanhToan { get; set; }
        public DateTime NgayThanhToan { get; set; }

        // Khóa ngoại liên kết với Hóa Đơn
        public int MaHoaDon { get; set; }
        public HoaDon HoaDon { get; set; } = null!;

        // Khóa ngoại liên kết với Phương Thức Thanh Toán
        public int MaPhuongThuc { get; set; }
        public PhuongThucThanhToan PhuongThucThanhToan { get; set; } = null!;
    }

}
