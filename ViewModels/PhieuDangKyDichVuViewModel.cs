using DACS_QuanLyPhongTro.Models;

namespace DACS_QuanLyPhongTro.ViewModels
{
    public class PhieuDangKyDichVuViewModel
    {
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }

        public List<DichVu> DichVus { get; set; } = new();
        public List<DichVuSelection> SelectedDichVus { get; set; } = new();
    }

    public class DichVuSelection
    {
        public int MaDichVu { get; set; }
        public bool IsSelected { get; set; }
        public int SoLuong { get; set; }
    }
}
