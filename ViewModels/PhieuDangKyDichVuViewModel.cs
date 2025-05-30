using DACS_QuanLyPhongTro.Models;

namespace DACS_QuanLyPhongTro.ViewModels
{
    public class PhieuDangKyDichVuViewModel
    {
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }

        public List<DichVu> DichVus { get; set; } = new();

        // Thay đổi SelectedDichVus thành List có số phần tử = số dịch vụ
        public List<DichVuSelection> SelectedDichVus { get; set; } = new();
    }

    public class DichVuSelection
    {
        public int MaDichVu { get; set; }

        // Sử dụng bool? để tránh binding lỗi khi checkbox không check (checkbox không gửi giá trị)
        public bool? IsSelected { get; set; } = false;

        public int SoLuong { get; set; } = 1;
    }
}
