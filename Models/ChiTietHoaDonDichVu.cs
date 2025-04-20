using System.ComponentModel.DataAnnotations.Schema;

namespace DACS_QuanLyPhongTro.Models
{
    [Table("ChiTietHoaDonDichVu")]
    public class ChiTietHoaDonDichVu
    {
        public int MaChiTiet { get; set; }

        // Khóa ngoại đến bảng Hóa Đơn
        public int MaHoaDon { get; set; }
        public HoaDon HoaDon { get; set; } = null!;

        // Khóa ngoại đến bảng Dịch Vụ
        public int MaDichVu { get; set; }
        public DichVu DichVu { get; set; } = null!;

        public int SoLuongPhieuDangKy { get; set; }
        
    }
}
