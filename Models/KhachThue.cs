using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    public class KhachThue
    {
        [Key]
        public int MaKhachThue { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;

        // Khóa ngoại liên kết với Phòng trọ
        public int MaPhong { get; set; }
        public PhongTro PhongTro { get; set; } = null!;

        // Liên kết với hợp đồng
        public List<HopDong> HopDongs { get; set; } = new();

        // Liên kết với phiếu ghi nhận sự cố
        public List<PhieuGhiNhanSuCo> PhieuGhiNhanSuCos { get; set; } = new();

        // Liên kết với phiếu đăng ký dịch vụ
        public List<PhieuDangKyDichVu> PhieuDangKyDichVus { get; set; } = new();

        // Liên kết với hóa đơn
        public List<HoaDon> HoaDons { get; set; } = new();
    }

}
