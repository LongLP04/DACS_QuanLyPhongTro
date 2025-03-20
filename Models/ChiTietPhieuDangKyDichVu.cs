namespace DACS_QuanLyPhongTro.Models
{
    public class ChiTietPhieuDangKyDichVu
    {
        public int MaChiTiet { get; set; }

        // Khóa ngoại đến bảng Phiếu Đăng Ký Dịch Vụ
        public int MaDangKyDichVu { get; set; }
        public PhieuDangKyDichVu PhieuDangKyDichVu { get; set; } = null!;

        // Khóa ngoại đến bảng Dịch Vụ
        public int MaDichVu { get; set; }
        public DichVu DichVu { get; set; } = null!;

        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
