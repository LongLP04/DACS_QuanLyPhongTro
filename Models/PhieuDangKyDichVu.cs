using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    public class PhieuDangKyDichVu
    {
        [Key]
        public int MaDangKyDichVu { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string TrangThai { get; set; } = string.Empty;

        // Khóa ngoại đến bảng KhachThue
        public int MaKhachThue { get; set; }
        public KhachThue KhachThue { get; set; }

        // Danh sách các dịch vụ đăng ký trong phiếu này
        public ICollection<ChiTietPhieuDangKyDichVu> ChiTietPhieuDangKyDichVus { get; set; } = new List<ChiTietPhieuDangKyDichVu>();
    }

}
