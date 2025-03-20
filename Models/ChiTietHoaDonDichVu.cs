namespace DACS_QuanLyPhongTro.Models
{
    public class ChiTietHoaDonDichVu
    {
        public int MaChiTiet { get; set; }

        // Khóa ngoại đến bảng Hóa Đơn
        public int MaHoaDon { get; set; }
        public HoaDon HoaDon { get; set; } = null!;

        // Khóa ngoại đến bảng Dịch Vụ
        public int MaDichVu { get; set; }
        public DichVu DichVu { get; set; } = null!;

        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
