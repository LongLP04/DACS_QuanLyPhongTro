using System.Collections.Generic;

namespace DACS_QuanLyPhongTro.ViewModels
{
    public class PhongTroViewModel
    {
        public int HoaDonChuaThanhToan { get; set; }
        public int DichVuDaSuDung { get; set; }
        public string ThongBaoMoiNhat { get; set; }
        public List<string> ThangTieuThu { get; set; }
        public List<int> DienTieuThu { get; set; }
        public List<int> NuocTieuThu { get; set; }
    }
}