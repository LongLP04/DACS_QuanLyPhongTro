using System.ComponentModel.DataAnnotations;

namespace DACS_QuanLyPhongTro.Models
{
    public class HoaDon
    {
        [Key]   
        public int MaHoaDon { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TienDien { get; set; }
        public decimal TienNuoc { get; set; }
        public decimal TienPhong { get; set; }
        public decimal TienDichVu { get; set; }
        public decimal TongTien { get; set; }
        public DateTime HanThanhToan { get; set; }
        public string TrangThai { get; set; } = string.Empty;

        // Khóa ngoại liên kết với Chỉ Số Điện Nước
        public int MaChiSo { get; set; }
        public ChiSoDienNuoc ChiSoDienNuoc { get; set; } = null!;

        // Khóa ngoại liên kết với Khách Thuê
        public int MaKhachThue { get; set; }
        public KhachThue KhachThue { get; set; } = null!;

        // Danh sách các phiếu thanh toán liên quan đến hóa đơn này
        public ICollection<PhieuThanhToan> PhieuThanhToans { get; set; } = new List<PhieuThanhToan>();

        // Danh sách chi tiết hóa đơn dịch vụ
        public ICollection<ChiTietHoaDonDichVu> ChiTietHoaDonDichVus { get; set; } = new List<ChiTietHoaDonDichVu>();
    }

}
